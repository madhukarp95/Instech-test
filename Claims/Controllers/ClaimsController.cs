using Claims.Models;
using Claims.Models.DTO;
using Claims.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly ILogger<ClaimsController> _logger;
    private readonly IClaimsService _claimsService;
    private readonly ICoverService _coverService;
    private readonly IChannel _channel;

    public ClaimsController(ILogger<ClaimsController> logger, IClaimsService claimsService,
        ICoverService coverService, IChannel channel)
    {
        _logger = logger;
        _claimsService = claimsService;
        _coverService = coverService;
        _channel = channel;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var result = await _claimsService.GetClaimsAsync();

        return result.Any() ? Ok(result) : NoContent();
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(ClaimDto claimDto)
    {
        Cover? cover = await _coverService.GetCoverAsync(claimDto.CoverId);

        if (cover is null)
        {
            _logger.LogWarning("Cover with ID {CoverId} not found.", claimDto.CoverId);

            return NotFound($"Cover with ID {claimDto.CoverId} not found.");
        }

        bool isValidClaim = claimDto.Created >= cover.StartDate && claimDto.Created <= cover.EndDate;

        if (isValidClaim)
        {
            var claim = await _claimsService.AddItemAsync(claimDto);
            await _channel.EnqueueAsync(new ChannelRequest(claim.Id, "POST", "CLAIM"));

            return Ok(claim);
        }

        return BadRequest("Claim date is not within the cover period");
    }

    [HttpDelete("{id:required}")]
    public async Task DeleteAsync(string id)
    {
        await _channel.EnqueueAsync(new ChannelRequest(id, "DELETE", "CLAIM"));
        await _claimsService.DeleteItemAsync(id);
    }

    [HttpGet("{id:required}")]
    public async Task<IActionResult> GetAsync(string id)
    {
        var result = await _claimsService.GetClaimAsync(id);

        return result is null ? NoContent() : Ok(result);
    }
}
