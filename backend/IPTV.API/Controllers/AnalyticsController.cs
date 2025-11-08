using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IPTV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly IRecommendationService _recommendationService;

    public AnalyticsController(IAnalyticsService analyticsService, IRecommendationService recommendationService)
    {
        _analyticsService = analyticsService;
        _recommendationService = recommendationService;
    }

    [HttpPost("viewing/start")]
    public async Task<ActionResult<ViewingHistory>> StartViewing([FromBody] StartViewingRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var viewingHistory = await _analyticsService.RecordViewingAsync(
            userId,
            request.ContentId,
            request.ChannelId,
            request.DeviceInfo
        );

        return Ok(viewingHistory);
    }

    [HttpPut("viewing/{id}/progress")]
    public async Task<ActionResult> UpdateViewingProgress(int id, [FromBody] UpdateProgressRequest request)
    {
        await _analyticsService.UpdateViewingProgressAsync(id, request.Progress, request.Completed);
        return NoContent();
    }

    [HttpGet("viewing/history")]
    public async Task<ActionResult<IEnumerable<ViewingHistory>>> GetViewingHistory([FromQuery] int count = 50)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var history = await _analyticsService.GetUserViewingHistoryAsync(userId, count);
        return Ok(history);
    }

    [HttpGet("viewing/continue")]
    public async Task<ActionResult<IEnumerable<Content>>> GetContinueWatching()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var content = await _analyticsService.GetContinueWatchingAsync(userId);
        return Ok(content);
    }

    [HttpGet("stats/genres")]
    public async Task<ActionResult<Dictionary<string, int>>> GetViewingStatsByGenre()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var stats = await _analyticsService.GetViewingStatsByGenreAsync(userId);
        return Ok(stats);
    }

    [HttpGet("stats/total-time")]
    public async Task<ActionResult<int>> GetTotalViewingTime()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var totalTime = await _analyticsService.GetTotalViewingTimeAsync(userId);
        return Ok(new { totalSeconds = totalTime });
    }

    [HttpGet("stats/popular/content")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Dictionary<string, int>>> GetMostWatchedContent([FromQuery] int count = 10)
    {
        var stats = await _analyticsService.GetMostWatchedContentAsync(count);
        return Ok(stats);
    }

    [HttpGet("stats/popular/channels")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Dictionary<string, int>>> GetMostWatchedChannels([FromQuery] int count = 10)
    {
        var stats = await _analyticsService.GetMostWatchedChannelsAsync(count);
        return Ok(stats);
    }

    [HttpGet("recommendations")]
    public async Task<ActionResult<IEnumerable<Content>>> GetRecommendations([FromQuery] int count = 10)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var recommendations = await _recommendationService.GetRecommendedContentAsync(userId, count);
        return Ok(recommendations);
    }

    [HttpGet("recommendations/similar/{contentId}")]
    public async Task<ActionResult<IEnumerable<Content>>> GetSimilarContent(int contentId, [FromQuery] int count = 10)
    {
        var similar = await _recommendationService.GetSimilarContentAsync(contentId, count);
        return Ok(similar);
    }

    [HttpGet("recommendations/channels")]
    public async Task<ActionResult<IEnumerable<Channel>>> GetRecommendedChannels([FromQuery] int count = 10)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var channels = await _recommendationService.GetRecommendedChannelsAsync(userId, count);
        return Ok(channels);
    }
}

public class StartViewingRequest
{
    public int? ContentId { get; set; }
    public int? ChannelId { get; set; }
    public string? DeviceInfo { get; set; }
}

public class UpdateProgressRequest
{
    public int Progress { get; set; }
    public bool Completed { get; set; }
}
