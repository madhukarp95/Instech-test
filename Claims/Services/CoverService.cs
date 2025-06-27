using Claims.Models;
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

        public async Task AddItemAsync(Cover item)
        {
            await _claimsContext.Covers.AddAsync(item);
            await _claimsContext.SaveChangesAsync();
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
