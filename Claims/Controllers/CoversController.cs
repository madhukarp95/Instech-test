using Claims.Models;
using Microsoft.AspNetCore.Mvc;
using Claims.Services.Channels;
using Claims.Models.DTO;
using Claims.Services.Coverage;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status204NoContent)]
public class CoversController : ControllerBase
{
    private readonly ICoverService _coverService;
    private readonly ILogger<CoversController> _logger;
    private readonly IChannelQueue _channel;

    public CoversController(ICoverService coverService, IChannelQueue channel, ILogger<CoversController> logger)
    {
        _coverService = coverService;
        _logger = logger;
        _channel = channel;
    }

    [HttpPost("compute")]
    public ActionResult ComputePremium(CoverDto coverDto)
    {
        try
        {
            decimal totalPremium = _coverService.ComputePremium(coverDto);

            return Ok(totalPremium);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var results = await _coverService.GetCoverAsync();

        return results.Any() ? Ok(results) : NoContent();
    }

    [HttpGet("{id:required}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        Cover? coverResponse = await _coverService.GetCoverAsync(id);

        return coverResponse is null ? NoContent() : Ok(coverResponse);
    }

    [HttpPost]
    public async Task<ActionResult<CoverDto>> CreateAsync(CoverDto coverDto)
    {
        var cover = await _coverService.AddItemAsync(coverDto);

        await _channel.EnqueueAsync(new ChannelRequest(cover.Id, "POST", "COVER"));

        return Ok(cover);
    }

    [HttpDelete("{id:required}")]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        await _channel.EnqueueAsync(new ChannelRequest(id, "DELETE", "COVER"));

        await _coverService.DeleteItemAsync(id);
        return NoContent();
    }
}
