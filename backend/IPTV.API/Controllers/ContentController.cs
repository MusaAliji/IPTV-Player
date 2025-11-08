using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPTV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentController : ControllerBase
{
    private readonly IContentService _contentService;
    private readonly ILogger<ContentController> _logger;

    public ContentController(IContentService contentService, ILogger<ContentController> logger)
    {
        _contentService = contentService;
        _logger = logger;
    }

    /// <summary>
    /// Get all content (catalog)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Content>>> GetCatalog()
    {
        try
        {
            var content = await _contentService.GetAllContentAsync();
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving content catalog");
            return StatusCode(500, "An error occurred while retrieving the catalog");
        }
    }

    /// <summary>
    /// Get content by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Content>> GetContent(int id)
    {
        try
        {
            var content = await _contentService.GetContentByIdAsync(id);
            if (content == null)
            {
                return NotFound();
            }
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving content with ID {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the content");
        }
    }

    /// <summary>
    /// Get content by type
    /// </summary>
    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<Content>>> GetContentByType(ContentType type)
    {
        try
        {
            var content = await _contentService.GetContentByTypeAsync(type);
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving content by type {Type}", type);
            return StatusCode(500, "An error occurred while retrieving content by type");
        }
    }

    /// <summary>
    /// Get content by genre
    /// </summary>
    [HttpGet("genre/{genre}")]
    public async Task<ActionResult<IEnumerable<Content>>> GetContentByGenre(string genre)
    {
        try
        {
            var content = await _contentService.GetContentByGenreAsync(genre);
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving content by genre {Genre}", genre);
            return StatusCode(500, "An error occurred while retrieving content by genre");
        }
    }

    /// <summary>
    /// Search content
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Content>>> SearchContent([FromQuery] string query)
    {
        try
        {
            var content = await _contentService.SearchContentAsync(query);
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching content with query {Query}", query);
            return StatusCode(500, "An error occurred while searching content");
        }
    }

    /// <summary>
    /// Get trending content
    /// </summary>
    [HttpGet("trending")]
    public async Task<ActionResult<IEnumerable<Content>>> GetTrending([FromQuery] int count = 10)
    {
        try
        {
            var content = await _contentService.GetTrendingContentAsync(count);
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving trending content");
            return StatusCode(500, "An error occurred while retrieving trending content");
        }
    }

    /// <summary>
    /// Get recent content
    /// </summary>
    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<Content>>> GetRecent([FromQuery] int count = 10)
    {
        try
        {
            var content = await _contentService.GetRecentContentAsync(count);
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent content");
            return StatusCode(500, "An error occurred while retrieving recent content");
        }
    }

    /// <summary>
    /// Create new content (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Content>> CreateContent([FromBody] Content content)
    {
        try
        {
            var created = await _contentService.CreateContentAsync(content);
            return CreatedAtAction(nameof(GetContent), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating content");
            return StatusCode(500, "An error occurred while creating content");
        }
    }

    /// <summary>
    /// Update content (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateContent(int id, [FromBody] Content content)
    {
        try
        {
            if (id != content.Id)
            {
                return BadRequest();
            }

            await _contentService.UpdateContentAsync(content);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating content with ID {Id}", id);
            return StatusCode(500, "An error occurred while updating content");
        }
    }

    /// <summary>
    /// Delete content (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteContent(int id)
    {
        try
        {
            await _contentService.DeleteContentAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting content with ID {Id}", id);
            return StatusCode(500, "An error occurred while deleting content");
        }
    }
}
