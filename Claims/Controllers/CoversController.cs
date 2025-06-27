using Claims.Models;
using Microsoft.AspNetCore.Mvc;
using Claims.Domain;
using Claims.Services.Interfaces;

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
    public async Task<ActionResult> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        decimal totalPremium = await Task.Run(() => PremiumCalculator.ComputePremium(startDate, endDate, coverType));
        return Ok(totalPremium);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var results = await _coverService.GetCoverAsync();

        return results is null ? NoContent() : Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        Cover? coverResponse = await _coverService.GetCoverAsync(id);

        return coverResponse is null ? NoContent() : Ok(coverResponse);

    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(Cover cover)
    {
        cover.Id = Guid.NewGuid().ToString();
        cover.Premium = PremiumCalculator.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);

        await _coverService.AddItemAsync(cover);

        await _auditer.AuditCover(cover.Id, "POST");

        return Ok(cover);
    }

    [HttpDelete("{id:required}")]
    public async Task DeleteAsync(string id)
    {
        await _auditer.AuditCover(id, "DELETE");

        // Find the cover by ID and remove it if it exists
        var cover = await _coverService.GetCoverAsync(id);

        if (cover is not null)
        {
            await _coverService.DeleteItemAsync(id);
        }
    }
}
