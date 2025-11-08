using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPTV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EPGController : ControllerBase
{
    private readonly IEPGService _epgService;

    public EPGController(IEPGService epgService)
    {
        _epgService = epgService;
    }

    [HttpGet("programs")]
    public async Task<ActionResult<IEnumerable<EPGProgram>>> GetAllPrograms()
    {
        var programs = await _epgService.GetAllProgramsAsync();
        return Ok(programs);
    }

    [HttpGet("programs/{id}")]
    public async Task<ActionResult<EPGProgram>> GetProgramById(int id)
    {
        var program = await _epgService.GetProgramByIdAsync(id);
        if (program == null)
        {
            return NotFound();
        }
        return Ok(program);
    }

    [HttpGet("programs/channel/{channelId}")]
    public async Task<ActionResult<IEnumerable<EPGProgram>>> GetProgramsByChannel(int channelId)
    {
        var programs = await _epgService.GetProgramsByChannelAsync(channelId);
        return Ok(programs);
    }

    [HttpGet("programs/current")]
    public async Task<ActionResult<IEnumerable<EPGProgram>>> GetCurrentPrograms()
    {
        var programs = await _epgService.GetCurrentProgramsAsync();
        return Ok(programs);
    }

    [HttpGet("programs/current/channel/{channelId}")]
    public async Task<ActionResult<EPGProgram>> GetCurrentProgramForChannel(int channelId)
    {
        var program = await _epgService.GetCurrentProgramForChannelAsync(channelId);
        if (program == null)
        {
            return NotFound();
        }
        return Ok(program);
    }

    [HttpGet("programs/range")]
    public async Task<ActionResult<IEnumerable<EPGProgram>>> GetProgramsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var programs = await _epgService.GetProgramsByDateRangeAsync(startDate, endDate);
        return Ok(programs);
    }

    [HttpPost("programs")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EPGProgram>> CreateProgram([FromBody] EPGProgram program)
    {
        var created = await _epgService.CreateProgramAsync(program);
        return CreatedAtAction(nameof(GetProgramById), new { id = created.Id }, created);
    }

    [HttpPut("programs/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateProgram(int id, [FromBody] EPGProgram program)
    {
        if (id != program.Id)
        {
            return BadRequest();
        }

        await _epgService.UpdateProgramAsync(program);
        return NoContent();
    }

    [HttpDelete("programs/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteProgram(int id)
    {
        await _epgService.DeleteProgramAsync(id);
        return NoContent();
    }

    [HttpGet("channels")]
    public async Task<ActionResult<IEnumerable<Channel>>> GetAllChannels()
    {
        var channels = await _epgService.GetAllChannelsAsync();
        return Ok(channels);
    }

    [HttpGet("channels/{id}")]
    public async Task<ActionResult<Channel>> GetChannelById(int id)
    {
        var channel = await _epgService.GetChannelByIdAsync(id);
        if (channel == null)
        {
            return NotFound();
        }
        return Ok(channel);
    }

    [HttpGet("channels/active")]
    public async Task<ActionResult<IEnumerable<Channel>>> GetActiveChannels()
    {
        var channels = await _epgService.GetActiveChannelsAsync();
        return Ok(channels);
    }
}
