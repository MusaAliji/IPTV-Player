using IPTV.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPTV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StreamingController : ControllerBase
{
    private readonly IContentService _contentService;
    private readonly IEPGService _epgService;

    public StreamingController(IContentService contentService, IEPGService epgService)
    {
        _contentService = contentService;
        _epgService = epgService;
    }

    [HttpGet("content/{id}/url")]
    public async Task<ActionResult<StreamUrlResponse>> GetContentStreamUrl(int id)
    {
        var content = await _contentService.GetContentByIdAsync(id);
        if (content == null)
        {
            return NotFound();
        }

        return Ok(new StreamUrlResponse
        {
            ContentId = content.Id,
            StreamUrl = content.StreamUrl,
            ContentType = content.Type.ToString(),
            Title = content.Title
        });
    }

    [HttpGet("channel/{id}/url")]
    public async Task<ActionResult<StreamUrlResponse>> GetChannelStreamUrl(int id)
    {
        var channel = await _epgService.GetChannelByIdAsync(id);
        if (channel == null)
        {
            return NotFound();
        }

        return Ok(new StreamUrlResponse
        {
            ChannelId = channel.Id,
            StreamUrl = channel.StreamUrl,
            ContentType = "LiveTV",
            Title = channel.Name
        });
    }

    [HttpGet("content/{id}/manifest")]
    public async Task<ActionResult> GetContentManifest(int id)
    {
        var content = await _contentService.GetContentByIdAsync(id);
        if (content == null)
        {
            return NotFound();
        }

        // This is a placeholder for HLS manifest generation
        // In a real implementation, you would generate or proxy the .m3u8 manifest
        return Ok(new
        {
            manifestUrl = $"{content.StreamUrl}/manifest.m3u8",
            type = "application/vnd.apple.mpegurl"
        });
    }

    [HttpGet("channel/{id}/manifest")]
    public async Task<ActionResult> GetChannelManifest(int id)
    {
        var channel = await _epgService.GetChannelByIdAsync(id);
        if (channel == null)
        {
            return NotFound();
        }

        // This is a placeholder for HLS manifest generation
        return Ok(new
        {
            manifestUrl = $"{channel.StreamUrl}/manifest.m3u8",
            type = "application/vnd.apple.mpegurl"
        });
    }
}

public class StreamUrlResponse
{
    public int? ContentId { get; set; }
    public int? ChannelId { get; set; }
    public required string StreamUrl { get; set; }
    public required string ContentType { get; set; }
    public required string Title { get; set; }
}
