using Microsoft.AspNetCore.Mvc;
using Claims.Models.DTO;
using Claims.Services.Coverage;
using Claims.Models.Cover;

namespace Claims.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType<decimal>(StatusCodes.Status200OK)]
public class CoversController : ControllerBase
{
    private readonly ICoverService _coverService;
    private readonly ILogger<CoversController> _logger;

    public CoversController(ICoverService coverService, ILogger<CoversController> logger)
    {
        _coverService = coverService;
        _logger = logger;
    }

    [HttpPost("compute")]
    public ActionResult ComputePremium(CoverDto coverEntity)
    {
        try
        {
            decimal totalPremium = _coverService.ComputePremium(coverEntity);

            return Ok(totalPremium);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    [HttpGet]
    public async Task<ActionResult> GetAsync()
    {
        var results = await _coverService.GetCoverAsync();

        return results.Any() ? Ok(results) : NoContent();
    }

    [HttpGet("{id:required}")]
    public async Task<ActionResult> GetAsync(string id)
    {
        Cover? coverResponse = await _coverService.GetCoverAsync(id);

        return coverResponse is null ? NoContent() : Ok(coverResponse);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(CoverDto CoverEntity)
    {
        var cover = await _coverService.AddItemAsync(CoverEntity);

        return Ok(cover);
    }

    [HttpDelete("{id:required}")]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        await _coverService.DeleteItemAsync(id);
        return NoContent();
    }
}
