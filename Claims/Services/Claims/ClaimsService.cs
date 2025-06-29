using Claims.Models.Claim;
using Claims.Models.DTO;
using Claims.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Claims.Services.Claims;

public class ClaimsService : IClaimsService
{
    private readonly ClaimsContext _claimsContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimsService"/> class.
    /// </summary>
    /// <param name="claimsContext"></param>
    public ClaimsService(ClaimsContext claimsContext)
    {
        _claimsContext = claimsContext;
    }

    // </inheritdoc>
    public async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        return await _claimsContext.Claims.ToListAsync();
    }

    // </inheritdoc>
    public async Task<Claim?> GetClaimAsync(string id)
    {
        return await _claimsContext.Claims.SingleOrDefaultAsync(claim => claim.Id == id);
    }

    // </inheritdoc>
    public async Task<Claim> AddItemAsync(ClaimDto model)
    {
        Claim claim = new()
        {
            Id = Guid.NewGuid().ToString(),
            CoverId = model.CoverId,
            Name = model.Name,
            Created = model.Created!.Value,
            DamageCost = model.DamageCost,
            Type = model.Type
        };

        await _claimsContext.Claims.AddAsync(claim);
        await _claimsContext.SaveChangesAsync();

        return claim;
    }

    // </inheritdoc>
    public async Task DeleteItemAsync(string id)
    {
        var claim = await GetClaimAsync(id);
        if (claim is not null)
        {
            _claimsContext.Claims.Remove(claim);
            await _claimsContext.SaveChangesAsync();
        }
    }
}
