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

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public void TestAuth_WhenAuthenticated_ShouldReturnOkWithUserInfo()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.Role, "User")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = _controller.TestAuth();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value;
        value.Should().NotBeNull();

        var valueType = value!.GetType();
        valueType.GetProperty("userId")?.GetValue(value).Should().Be("1");
        valueType.GetProperty("username")?.GetValue(value).Should().Be("testuser");
        valueType.GetProperty("role")?.GetValue(value).Should().Be("User");
        valueType.GetProperty("isAuthenticated")?.GetValue(value).Should().Be(true);
    }

    [Fact]
    public async Task Register_WithValidRequest_ShouldReturnOkWithAuthResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "newuser",
            Email = "newuser@test.com",
            Password = "Password123!",
            FullName = "New User"
        };

        var createdUser = TestDataBuilder.CreateTestUser(1, "newuser");
        createdUser.Email = request.Email;
        createdUser.FullName = request.FullName;

        const string token = "test.jwt.token";

        _authServiceMock.Setup(s => s.RegisterAsync(
            request.Username,
            request.Email,
            request.Password,
            request.FullName))
            .ReturnsAsync(createdUser);

        _authServiceMock.Setup(s => s.GenerateJwtToken(createdUser))
            .Returns(token);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var authResponse = okResult.Value.Should().BeOfType<AuthResponse>().Subject;

        authResponse.UserId.Should().Be(1);
        authResponse.Username.Should().Be("newuser");
        authResponse.Email.Should().Be(request.Email);
        authResponse.FullName.Should().Be(request.FullName);
        authResponse.Token.Should().Be(token);

        _authServiceMock.Verify(s => s.RegisterAsync(
            request.Username,
            request.Email,
            request.Password,
            request.FullName), Times.Once);
        _authServiceMock.Verify(s => s.GenerateJwtToken(createdUser), Times.Once);
    }

    [Fact]
    public async Task Register_WhenUsernameExists_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "existinguser",
            Email = "test@test.com",
            Password = "Password123!",
            FullName = "Test User"
        };

        _authServiceMock.Setup(s => s.RegisterAsync(
            request.Username,
            request.Email,
            request.Password,
            request.FullName))
            .ThrowsAsync(new InvalidOperationException("Username already exists"));

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var value = badRequestResult.Value;
        value.Should().NotBeNull();

        var valueType = value!.GetType();
        var message = valueType.GetProperty("message")?.GetValue(value) as string;
        message.Should().Be("Username already exists");
    }

    [Fact]
    public async Task Register_WhenEmailExists_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "newuser",
            Email = "existing@test.com",
            Password = "Password123!",
            FullName = "Test User"
        };

        _authServiceMock.Setup(s => s.RegisterAsync(
            request.Username,
            request.Email,
            request.Password,
            request.FullName))
            .ThrowsAsync(new InvalidOperationException("Email already exists"));

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var value = badRequestResult.Value;
        value.Should().NotBeNull();

        var valueType = value!.GetType();
        var message = valueType.GetProperty("message")?.GetValue(value) as string;
        message.Should().Be("Email already exists");
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkWithAuthResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "Password123!"
        };

        var user = TestDataBuilder.CreateTestUser(1, "testuser");
        const string token = "test.jwt.token";

        _authServiceMock.Setup(s => s.LoginAsync(request.Username, request.Password))
            .ReturnsAsync((user, token));

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var authResponse = okResult.Value.Should().BeOfType<AuthResponse>().Subject;

        authResponse.UserId.Should().Be(1);
        authResponse.Username.Should().Be("testuser");
        authResponse.Token.Should().Be(token);

        _authServiceMock.Verify(s => s.LoginAsync(request.Username, request.Password), Times.Once);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "WrongPassword"
        };

        _authServiceMock.Setup(s => s.LoginAsync(request.Username, request.Password))
            .ReturnsAsync((ValueTuple<User, string>?)null);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var unauthorizedResult = result.Result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        var value = unauthorizedResult.Value;
        value.Should().NotBeNull();

        var valueType = value!.GetType();
        var message = valueType.GetProperty("message")?.GetValue(value) as string;
        message.Should().Be("Invalid username or password");

        _authServiceMock.Verify(s => s.LoginAsync(request.Username, request.Password), Times.Once);
    }

    [Fact]
    public async Task Login_WhenUserDoesNotExist_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "nonexistent",
            Password = "Password123!"
        };

        _authServiceMock.Setup(s => s.LoginAsync(request.Username, request.Password))
            .ReturnsAsync((ValueTuple<User, string>?)null);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var unauthorizedResult = result.Result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        var value = unauthorizedResult.Value;
        value.Should().NotBeNull();

        var valueType = value!.GetType();
        var message = valueType.GetProperty("message")?.GetValue(value) as string;
        message.Should().Be("Invalid username or password");

        _authServiceMock.Verify(s => s.LoginAsync(request.Username, request.Password), Times.Once);
    }
}
