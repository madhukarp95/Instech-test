using Claims.Domain;
using Claims.Models;
using Claims.Models.DTO;
using Claims.Persistance;
using Claims.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Claims.Services
{
    public class CoverService : ICoverService
    {
        private readonly ClaimsContext _claimsContext;

        public CoverService(ClaimsContext claimsContext)
        {
            _claimsContext = claimsContext;
        }

        public async Task<Cover> AddItemAsync(CoverDto coverDto)
        {
            decimal Premium = await Task.Run(() =>
                PremiumCalculator.ComputePremium(coverDto.StartDate!.Value, coverDto.EndDate!.Value, coverDto.Type));

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

        public async Task DeleteItemAsync(string id)
        {
            var cover = await GetCoverAsync(id);

            if (cover is not null)
            {
                _claimsContext.Covers.Remove(cover);
                await _claimsContext.SaveChangesAsync();
            }
        }

        public async Task<Cover?> GetCoverAsync(string id)
        {
            return await _claimsContext.Covers.SingleOrDefaultAsync(cover => cover.Id == id);
        }

        public async Task<IEnumerable<Cover>> GetCoverAsync()
        {
            return await _claimsContext.Covers.ToListAsync();
        }
    }
}
