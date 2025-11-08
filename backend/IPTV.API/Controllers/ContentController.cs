using IPTV.Core.Entities;
using IPTV.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IPTV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentController : ControllerBase
{
    private readonly IPTVDbContext _context;
    private readonly ILogger<ContentController> _logger;

    public ContentController(IPTVDbContext context, ILogger<ContentController> logger)
    {
        _context = context;
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
            var content = await _context.Contents
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

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
            var content = await _context.Contents.FindAsync(id);

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
            var content = await _context.Contents
                .Where(c => c.Type == type)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving content by type {Type}", type);
            return StatusCode(500, "An error occurred while retrieving content by type");
        }
    }
}
