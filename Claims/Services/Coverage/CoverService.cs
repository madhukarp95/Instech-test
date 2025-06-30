using Claims.Models.Channel;
using Claims.Models.Cover;
using Claims.Models.DTO;
using Claims.Persistence;
using Claims.Services.Channels;
using Claims.Services.PremiumCalculator;
using Microsoft.EntityFrameworkCore;

namespace Claims.Services.Coverage;

public class CoverService : ICoverService
{
    private readonly ClaimsContext _claimsContext;
    private readonly IPremiumCalculatorFactory _premiumCalculator;
    private readonly IChannelQueue _channel;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoverService"/> class.
    /// </summary>
    /// <param name="claimsContext"></param>
    /// <param name="premiumCalculator"></param>
    /// <param name="channel"></param>
    public CoverService(ClaimsContext claimsContext, IPremiumCalculatorFactory premiumCalculator, IChannelQueue channel)
    {
        _claimsContext = claimsContext;
        _premiumCalculator = premiumCalculator;
        _channel = channel;
    }

    // </inheritdoc>
    public async Task<Cover> AddItemAsync(CoverDto item)
    {
        decimal Premium = _premiumCalculator.GetCalculator(item.Type)
                                .CalculatePremiumForCoverType(item.StartDate!.Value, item.EndDate!.Value);

        Cover cover = new()
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = item.StartDate!.Value,
            EndDate = item.EndDate!.Value,
            Type = item.Type,
            Premium = Premium
        };

        await _claimsContext.Covers.AddAsync(cover);
        await _claimsContext.SaveChangesAsync();

        await _channel.EnqueueAsync(new ChannelRequest(cover.Id, Constants.HttpPost, Constants.CoverType));

        return cover;
    }

    // </inheritdoc>
    public decimal ComputePremium(CoverDto item)
    {
        return _premiumCalculator.GetCalculator(item.Type)
                                .CalculatePremiumForCoverType(item.StartDate!.Value, item.EndDate!.Value);
    }

    // </inheritdoc>
    public async Task DeleteItemAsync(string id)
    {
        var cover = await GetCoverAsync(id);

        if (cover is not null)
        {
            _claimsContext.Covers.Remove(cover);
            await _claimsContext.SaveChangesAsync();

            await _channel.EnqueueAsync(new ChannelRequest(id, Constants.HttpDelete, Constants.CoverType));
        }
    }

    // </inheritdoc>
    public async Task<Cover?> GetCoverAsync(string id)
    {
        return await _claimsContext.Covers.AsNoTracking().SingleOrDefaultAsync(cover => cover.Id == id);
    }

    // </inheritdoc>
    public async Task<IEnumerable<Cover>> GetCoverAsync()
    {
        return await _claimsContext.Covers.AsNoTracking().ToListAsync();
    }
}
