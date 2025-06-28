using Claims.Models;
using Microsoft.AspNetCore.Mvc;
using Claims.Domain;
using Claims.Services.Interfaces;
using Claims.Models.DTO;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ICoverService _coverService;
    private readonly ILogger<CoversController> _logger;
    private readonly IAuditer _auditer;

    public CoversController(ICoverService coverService, IAuditer auditer, ILogger<CoversController> logger)
    {
        _coverService = coverService;
        _logger = logger;
        _auditer = auditer;
    }

    [HttpPost("compute")]
    public async Task<ActionResult> ComputePremiumAsync(CoverDto coverDto)
    {
        decimal totalPremium = await Task.Run(() =>
            PremiumCalculator.ComputePremium(coverDto.StartDate!.Value, coverDto.EndDate!.Value, coverDto.Type));

        return Ok(totalPremium);
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

        await _auditer.AuditCover(cover.Id, "POST");

        return Ok(cover);
    }

    [HttpDelete("{id:required}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        await _auditer.AuditCover(id, "DELETE");

        // Find the cover by ID and remove it if it exists
        var cover = await _coverService.GetCoverAsync(id);

        if (cover is null)
        {
            return NotFound();
        }

        await _coverService.DeleteItemAsync(id);
        return NoContent();
    }
}
