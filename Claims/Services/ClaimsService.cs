using Claims.Models;
using Claims.Persistance;
using Claims.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Claims.Services
{
    public class ClaimsService : IClaimsService
    {
        private readonly ClaimsContext _claimsContext;

        public ClaimsService(ClaimsContext claimsContext)
        {
            _claimsContext = claimsContext;
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync()
        {
            return await _claimsContext.Claims.ToListAsync();
        }

        public async Task<Claim?> GetClaimAsync(string id)
        {
            return await _claimsContext.Claims.SingleOrDefaultAsync(claim => claim.Id == id);
        }

        public async Task AddItemAsync(Claim item)
        {
            await _claimsContext.Claims.AddAsync(item);
            await _claimsContext.SaveChangesAsync();
        }

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
}
