using Claims.Persistance;
using Claims.Models;
using Microsoft.AspNetCore.Mvc;
using Claims.Services.Interfaces;


namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly ILogger<ClaimsController> _logger;
    private readonly ClaimsContext _claimsContext;
    private readonly IAuditer _auditer;

    public ClaimsController(ILogger<ClaimsController> logger, ClaimsContext claimsContext, IAuditer auditer)
    {
        _logger = logger;
        _claimsContext = claimsContext;
        _auditer = auditer;
    }

    [HttpGet]
    public async Task<IEnumerable<Claim>> GetAsync()
    {
        return await _claimsContext.GetClaimsAsync();
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(Claim claim)
    {
        claim.Id = Guid.NewGuid().ToString();
        await _claimsContext.AddItemAsync(claim);
        await _auditer.AuditClaim(claim.Id, "POST");
        return Ok(claim);
    }

    [HttpDelete("{id:required}")]
    public async Task DeleteAsync(string id)
    {
        await _auditer.AuditClaim(id, "DELETE");
        await _claimsContext.DeleteItemAsync(id);
    }

    [HttpGet("{id:required}")]
    public async Task<Claim> GetAsync(string id)
    {
        return await _claimsContext.GetClaimAsync(id);
    }
}
