using FluentAssertions;
using IPTV.API.Controllers;
using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using IPTV.Tests.Unit.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace IPTV.Tests.Unit.Controllers;

public class UserControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new UserController(_authServiceMock.Object);
    }

    private void SetupUserClaims(int userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.Role, "User")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task GetProfile_WhenUserExists_ShouldReturnOkWithUserProfile()
    {
        // Arrange
        const int userId = 1;
        SetupUserClaims(userId);

        var user = TestDataBuilder.CreateTestUser(userId);
        _authServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetProfile();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedUser = okResult.Value.Should().BeOfType<User>().Subject;
        returnedUser.Id.Should().Be(userId);
        returnedUser.PasswordHash.Should().BeEmpty(); // Password should be cleared
        _authServiceMock.Verify(s => s.GetUserByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetProfile_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        const int userId = 999;
        SetupUserClaims(userId);

        _authServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await _controller.GetProfile();

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetProfile_WhenNoUserIdClaim_ShouldReturnUnauthorized()
    {
        // Arrange - Setup controller context with no claims
        var claims = new List<Claim>(); // Empty claims list
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.GetProfile();

        // Assert
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task UpdateProfile_WhenValid_ShouldReturnNoContent()
    {
        // Arrange
        const int userId = 1;
        SetupUserClaims(userId);

        var existingUser = TestDataBuilder.CreateTestUser(userId);
        var updatedUser = TestDataBuilder.CreateTestUser(userId);
        updatedUser.FullName = "Updated Name";
        updatedUser.Email = "updated@test.com";

        _authServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(existingUser);
        _authServiceMock.Setup(s => s.UpdateUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateProfile(updatedUser);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _authServiceMock.Verify(s => s.UpdateUserAsync(It.Is<User>(u =>
            u.FullName == "Updated Name" && u.Email == "updated@test.com")), Times.Once);
    }

    [Fact]
    public async Task UpdateProfile_WhenUserIdMismatch_ShouldReturnForbid()
    {
        // Arrange
        const int userId = 1;
        SetupUserClaims(userId);

        var updatedUser = TestDataBuilder.CreateTestUser(2); // Different user ID

        // Act
        var result = await _controller.UpdateProfile(updatedUser);

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _authServiceMock.Verify(s => s.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateProfile_WhenUserNotFound_ShouldReturnNotFound()
    {
        // Arrange
        const int userId = 1;
        SetupUserClaims(userId);

        var updatedUser = TestDataBuilder.CreateTestUser(userId);
        _authServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await _controller.UpdateProfile(updatedUser);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ChangePassword_WhenValid_ShouldReturnOk()
    {
        // Arrange
        const int userId = 1;
        SetupUserClaims(userId);

        var request = new ChangePasswordRequest
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!"
        };

        _authServiceMock.Setup(s => s.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.ChangePassword(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value;
        value.Should().NotBeNull();

        var valueType = value!.GetType();
        var message = valueType.GetProperty("message")?.GetValue(value) as string;
        message.Should().Be("Password changed successfully");
    }

    [Fact]
    public async Task ChangePassword_WhenCurrentPasswordIncorrect_ShouldReturnBadRequest()
    {
        // Arrange
        const int userId = 1;
        SetupUserClaims(userId);

        var request = new ChangePasswordRequest
        {
            CurrentPassword = "WrongPassword",
            NewPassword = "NewPassword123!"
        };

        _authServiceMock.Setup(s => s.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.ChangePassword(request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var value = badRequestResult.Value;
        value.Should().NotBeNull();

        var valueType = value!.GetType();
        var message = valueType.GetProperty("message")?.GetValue(value) as string;
        message.Should().Be("Current password is incorrect");
    }

    [Fact]
    public async Task GetPreferences_WhenPreferencesExist_ShouldReturnOk()
    {
        // Arrange
        const int userId = 1;
        SetupUserClaims(userId);

        var preferences = TestDataBuilder.CreateTestUserPreference(1, userId);
        _authServiceMock.Setup(s => s.GetUserPreferencesAsync(userId)).ReturnsAsync(preferences);

        // Act
        var result = await _controller.GetPreferences();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPreferences = okResult.Value.Should().BeOfType<UserPreference>().Subject;
        returnedPreferences.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetPreferences_WhenPreferencesNotFound_ShouldReturnNotFound()
    {
        // Arrange
        const int userId = 1;
        SetupUserClaims(userId);

        _authServiceMock.Setup(s => s.GetUserPreferencesAsync(userId)).ReturnsAsync((UserPreference?)null);

        // Act
        var result = await _controller.GetPreferences();

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdatePreferences_WhenValid_ShouldReturnNoContent()
    {
        // Arrange
        const int userId = 1;
        SetupUserClaims(userId);

        var preferences = TestDataBuilder.CreateTestUserPreference(1, userId);
        preferences.Language = "es";

        _authServiceMock.Setup(s => s.UpdateUserPreferencesAsync(preferences)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdatePreferences(preferences);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _authServiceMock.Verify(s => s.UpdateUserPreferencesAsync(preferences), Times.Once);
    }

    [Fact]
    public async Task UpdatePreferences_WhenUserIdMismatch_ShouldReturnForbid()
    {
        // Arrange
        const int userId = 1;
        SetupUserClaims(userId);

        var preferences = TestDataBuilder.CreateTestUserPreference(1, 2); // Different user ID

        // Act
        var result = await _controller.UpdatePreferences(preferences);

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _authServiceMock.Verify(s => s.UpdateUserPreferencesAsync(It.IsAny<UserPreference>()), Times.Never);
    }

    [Fact]
    public async Task GetUserById_WhenUserExists_ShouldReturnOkWithUser()
    {
        // Arrange
        const int userId = 1;
        var user = TestDataBuilder.CreateTestUser(userId);
        _authServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetUserById(userId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedUser = okResult.Value.Should().BeOfType<User>().Subject;
        returnedUser.Id.Should().Be(userId);
        returnedUser.PasswordHash.Should().BeEmpty(); // Password should be cleared
    }

    [Fact]
    public async Task GetUserById_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        const int userId = 999;
        _authServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await _controller.GetUserById(userId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }
}
