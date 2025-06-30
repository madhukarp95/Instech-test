using Claims.Models.DTO;
using Claims.Services.Claims;
using Claims.Services.Coverage;
using Claims.Services.Channels;
using Microsoft.AspNetCore.Mvc;
using Claims.Models.Cover;
using Claims.Models.Channel;


namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status204NoContent)]
public class ClaimsController : ControllerBase
{
    private readonly ILogger<ClaimsController> _logger;
    private readonly IClaimsService _claimsService;
    private readonly ICoverService _coverService;

    public ClaimsController(ILogger<ClaimsController> logger, IClaimsService claimsService,
        ICoverService coverService)
    {
        _logger = logger;
        _claimsService = claimsService;
        _coverService = coverService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var result = await _claimsService.GetClaimsAsync();

        return result.Any() ? Ok(result) : NoContent();
    }

    [HttpGet("{id:required}")]
    public async Task<ActionResult> GetAsync(string id)
    {
        var result = await _claimsService.GetClaimAsync(id);

        return result is null ? NoContent() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(ClaimDto claimEntity)
    {
        Cover? cover = await _coverService.GetCoverAsync(claimEntity.CoverId);

        if (cover is null)
        {
            _logger.LogWarning("Cover with ID {CoverId} not found.", claimEntity.CoverId);

            return NotFound($"Cover with ID {claimEntity.CoverId} not found.");
        }

        bool isValidClaim = claimEntity.Created >= cover.StartDate && claimEntity.Created <= cover.EndDate;

        if (isValidClaim)
        {
            var claim = await _claimsService.AddItemAsync(claimEntity);

            return Ok(claim);
        }

        return BadRequest("Claim date is not within the cover period");
    }

    [HttpDelete("{id:required}")]
    public async Task DeleteAsync(string id)
    {
        await _claimsService.DeleteItemAsync(id);
    }
}
