using Claims.Models.Cover;
using Claims.Models.DTO;
using Claims.Persistance;
using Claims.Services.PremiumCalculator;
using Microsoft.EntityFrameworkCore;

namespace Claims.Services.Coverage;

public class CoverService : ICoverService
{
    private readonly ClaimsContext _claimsContext;
    private readonly IPremiumCalculatorFactory _premiumCalculator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoverService"/> class.
    /// </summary>
    /// <param name="claimsContext"></param>
    /// <param name="premiumCalculator"></param>
    public CoverService(ClaimsContext claimsContext, IPremiumCalculatorFactory premiumCalculator)
    {
        _claimsContext = claimsContext;
        _premiumCalculator = premiumCalculator;
    }

    // </inheritdoc>
    public async Task<Cover> AddItemAsync(CoverDto coverDto)
    {
        decimal Premium = _premiumCalculator.GetCalculator(coverDto.Type)
                                .CalculatePremiumForCoverType(coverDto.StartDate!.Value, coverDto.EndDate!.Value);

        Cover cover = new()
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = coverDto.StartDate!.Value,
            EndDate = coverDto.EndDate!.Value,
            Type = coverDto.Type,
            Premium = Premium
        };

        await _claimsContext.Covers.AddAsync(cover);
        await _claimsContext.SaveChangesAsync();

        return cover;
    }

    // </inheritdoc>
    public decimal ComputePremium(CoverDto coverDto)
    {
        return _premiumCalculator.GetCalculator(coverDto.Type)
                                .CalculatePremiumForCoverType(coverDto.StartDate!.Value, coverDto.EndDate!.Value);
    }

    // </inheritdoc>
    public async Task DeleteItemAsync(string id)
    {
        var cover = await GetCoverAsync(id);

        if (cover is not null)
        {
            _claimsContext.Covers.Remove(cover);
            await _claimsContext.SaveChangesAsync();
        }
    }

    // </inheritdoc>
    public async Task<Cover?> GetCoverAsync(string id)
    {
        return await _claimsContext.Covers.SingleOrDefaultAsync(cover => cover.Id == id);
    }

    // </inheritdoc>
    public async Task<IEnumerable<Cover>> GetCoverAsync()
    {
        return await _claimsContext.Covers.ToListAsync();
    }
}
