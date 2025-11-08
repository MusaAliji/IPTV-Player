using FluentAssertions;
using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using IPTV.Infrastructure.Services;
using IPTV.Tests.Unit.Helpers;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace IPTV.Tests.Unit.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<User>> _userRepositoryMock;
    private readonly Mock<IRepository<UserPreference>> _preferencesRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IRepository<User>>();
        _preferencesRepositoryMock = new Mock<IRepository<UserPreference>>();
        _configurationMock = new Mock<IConfiguration>();

        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserPreferences).Returns(_preferencesRepositoryMock.Object);

        // Setup JWT configuration
        var jwtSection = new Mock<IConfigurationSection>();
        jwtSection.Setup(s => s["SecretKey"]).Returns("Test-Secret-Key-For-JWT-Token-Generation-Min-32-Characters");
        jwtSection.Setup(s => s["Issuer"]).Returns("TestIssuer");
        jwtSection.Setup(s => s["Audience"]).Returns("TestAudience");
        jwtSection.Setup(s => s["ExpirationMinutes"]).Returns("60");
        _configurationMock.Setup(c => c.GetSection("JwtSettings")).Returns(jwtSection.Object);

        _authService = new AuthService(_unitOfWorkMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync((User?)null);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _authService.RegisterAsync("newuser", "new@test.com", "Test@123", "New User");

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be("newuser");
        result.Email.Should().Be("new@test.com");
        result.FullName.Should().Be("New User");
        result.Role.Should().Be(UserRole.User);
        result.IsActive.Should().BeTrue();
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _preferencesRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserPreference>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenUserExists()
    {
        // Arrange
        var existingUser = TestDataBuilder.CreateTestUser();
        _userRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _authService.RegisterAsync("testuser", "test@test.com", "Test@123"));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUserAndToken_WhenCredentialsAreValid()
    {
        // Arrange
        var password = "Test@123";
        var hashedPassword = _authService.HashPassword(password);
        var user = TestDataBuilder.CreateTestUser(1, "testuser");
        user.PasswordHash = hashedPassword;

        _userRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _authService.LoginAsync("testuser", password);

        // Assert
        result.Should().NotBeNull();
        result.Value.user.Should().Be(user);
        result.Value.token.Should().NotBeNullOrEmpty();
        user.LastLoginAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync("nonexistent", "password");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsInvalid()
    {
        // Arrange
        var user = TestDataBuilder.CreateTestUser();
        user.PasswordHash = _authService.HashPassword("CorrectPassword");
        _userRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync("testuser", "WrongPassword");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserIsInactive()
    {
        // Arrange
        var user = TestDataBuilder.CreateTestUser();
        user.IsActive = false;
        _userRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync("testuser", "password");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void HashPassword_ShouldGenerateValidHash()
    {
        // Arrange
        var password = "TestPassword@123";

        // Act
        var hash = _authService.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        var hashBytes = Convert.FromBase64String(hash);
        hashBytes.Length.Should().Be(48); // 16 bytes salt + 32 bytes hash
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrue_WhenPasswordMatches()
    {
        // Arrange
        var password = "TestPassword@123";
        var hash = _authService.HashPassword(password);

        // Act
        var result = _authService.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_WhenPasswordDoesNotMatch()
    {
        // Arrange
        var password = "TestPassword@123";
        var hash = _authService.HashPassword(password);

        // Act
        var result = _authService.VerifyPassword("WrongPassword", hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GenerateJwtToken_ShouldGenerateValidToken()
    {
        // Arrange
        var user = TestDataBuilder.CreateTestUser(1, "testuser", UserRole.Admin);

        // Act
        var token = _authService.GenerateJwtToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT has 3 parts
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnTrue_WhenCurrentPasswordIsCorrect()
    {
        // Arrange
        var currentPassword = "Current@123";
        var newPassword = "New@123";
        var user = TestDataBuilder.CreateTestUser();
        user.PasswordHash = _authService.HashPassword(currentPassword);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _authService.ChangePasswordAsync(1, currentPassword, newPassword);

        // Assert
        result.Should().BeTrue();
        _authService.VerifyPassword(newPassword, user.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnFalse_WhenCurrentPasswordIsIncorrect()
    {
        // Arrange
        var user = TestDataBuilder.CreateTestUser();
        user.PasswordHash = _authService.HashPassword("Current@123");
        _userRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _authService.ChangePasswordAsync(1, "Wrong@123", "New@123");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenExists()
    {
        // Arrange
        var user = TestDataBuilder.CreateTestUser();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _authService.GetUserByIdAsync(1);

        // Assert
        result.Should().Be(user);
    }

    [Fact]
    public async Task GetUserPreferencesAsync_ShouldReturnPreferences_WhenExists()
    {
        // Arrange
        var preferences = TestDataBuilder.CreateTestUserPreference();
        _preferencesRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserPreference, bool>>>()))
            .ReturnsAsync(preferences);

        // Act
        var result = await _authService.GetUserPreferencesAsync(1);

        // Assert
        result.Should().Be(preferences);
    }

    [Fact]
    public async Task UpdateUserPreferencesAsync_ShouldUpdatePreferences()
    {
        // Arrange
        var preferences = TestDataBuilder.CreateTestUserPreference();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _authService.UpdateUserPreferencesAsync(preferences);

        // Assert
        preferences.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _preferencesRepositoryMock.Verify(r => r.Update(preferences), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
