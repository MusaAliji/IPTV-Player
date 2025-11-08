using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IPTV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IAuthService _authService;

    public UserController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<User>> GetProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        // Don't return password hash
        user.PasswordHash = string.Empty;
        return Ok(user);
    }

    [HttpPut("profile")]
    public async Task<ActionResult> UpdateProfile([FromBody] User updatedUser)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        if (userId != updatedUser.Id)
        {
            return Forbid();
        }

        var existingUser = await _authService.GetUserByIdAsync(userId);
        if (existingUser == null)
        {
            return NotFound();
        }

        // Only update allowed fields
        existingUser.FullName = updatedUser.FullName;
        existingUser.Email = updatedUser.Email;

        await _authService.UpdateUserAsync(existingUser);
        return NoContent();
    }

    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var success = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
        if (!success)
        {
            return BadRequest(new { message = "Current password is incorrect" });
        }

        return Ok(new { message = "Password changed successfully" });
    }

    [HttpGet("preferences")]
    public async Task<ActionResult<UserPreference>> GetPreferences()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var preferences = await _authService.GetUserPreferencesAsync(userId);
        if (preferences == null)
        {
            return NotFound();
        }

        return Ok(preferences);
    }

    [HttpPut("preferences")]
    public async Task<ActionResult> UpdatePreferences([FromBody] UserPreference preferences)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        if (userId != preferences.UserId)
        {
            return Forbid();
        }

        await _authService.UpdateUserPreferencesAsync(preferences);
        return NoContent();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var user = await _authService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.PasswordHash = string.Empty;
        return Ok(user);
    }
}

public class ChangePasswordRequest
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}
