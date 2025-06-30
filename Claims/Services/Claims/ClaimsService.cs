using Claims.Models.Channel;
using Claims.Models.Claim;
using Claims.Models.DTO;
using Claims.Persistence;
using Claims.Services.Channels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

namespace Claims.Services.Claims;

public class ClaimsService : IClaimsService
{
    private readonly ClaimsContext _claimsContext;
    private readonly IChannelQueue _channel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimsService"/> class.
    /// </summary>
    /// <param name="claimsContext"></param>
    /// <param name="channel"></param>
    public ClaimsService(ClaimsContext claimsContext, IChannelQueue channel)
    {
        _claimsContext = claimsContext;
        _channel = channel;
    }

    // </inheritdoc>
    public async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        return await _claimsContext.Claims.AsNoTracking().ToListAsync();
    }

    // </inheritdoc>
    public async Task<Claim?> GetClaimAsync(string id)
    {
        return await _claimsContext.Claims.AsNoTracking().SingleOrDefaultAsync(claim => claim.Id == id);
    }

    // </inheritdoc>
    public async Task<Claim> AddItemAsync(ClaimDto item)
    {
        Claim claim = new()
        {
            Id = Guid.NewGuid().ToString(),
            CoverId = item.CoverId,
            Name = item.Name,
            Created = item.Created!.Value,
            DamageCost = item.DamageCost,
            Type = item.Type
        };

        await _claimsContext.Claims.AddAsync(claim);
        await _claimsContext.SaveChangesAsync();

        await _channel.EnqueueAsync(new ChannelRequest(claim.Id, Constants.HttpPost, Constants.ClaimType));

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

            await _channel.EnqueueAsync(new ChannelRequest(id, Constants.HttpDelete, Constants.ClaimType));
        }
    }
}
