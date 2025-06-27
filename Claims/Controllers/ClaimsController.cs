using Claims.Models;
using Microsoft.AspNetCore.Mvc;
using Claims.Services.Interfaces;


namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly ILogger<ClaimsController> _logger;
    private readonly IAuditer _auditer;
    private readonly IClaimsService _claimsService;

    public ClaimsController(ILogger<ClaimsController> logger, IAuditer auditer, IClaimsService claimsService)
    {
        _logger = logger;
        _auditer = auditer;
        _claimsService = claimsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var result = await _claimsService.GetClaimsAsync();

        return result.Any() ? Ok(result) : NoContent();
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(Claim claim)
    {
        claim.Id = Guid.NewGuid().ToString();
        await _claimsService.AddItemAsync(claim);
        await _auditer.AuditClaim(claim.Id, "POST");
        return Ok(claim);
    }

    [HttpDelete("{id:required}")]
    public async Task DeleteAsync(string id)
    {
        await _auditer.AuditClaim(id, "DELETE");
        await _claimsService.DeleteItemAsync(id);
    }

    [HttpGet("{id:required}")]
    public async Task<IActionResult> GetAsync(string id)
    {
        var result = await _claimsService.GetClaimAsync(id);

        return result is null ? NoContent() : Ok(result);
    }
}
