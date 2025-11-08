using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IPTV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("test")]
    [Authorize]
    public ActionResult TestAuth()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            message = "Authentication successful!",
            userId = userId,
            username = username,
            role = role,
            allClaims = claims,
            isAuthenticated = User.Identity?.IsAuthenticated ?? false
        });
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var user = await _authService.RegisterAsync(
                request.Username,
                request.Email,
                request.Password,
                request.FullName
            );

            var token = _authService.GenerateJwtToken(user);

            return Ok(new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                Token = token
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Username, request.Password);

        if (result == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var (user, token) = result.Value;

        return Ok(new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            Token = token
        });
    }
}

public class RegisterRequest
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? FullName { get; set; }
}

public class LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class AuthResponse
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? FullName { get; set; }
    public required string Role { get; set; }
    public required string Token { get; set; }
}
