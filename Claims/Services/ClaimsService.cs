using Claims.Models;
using Claims.Models.DTO;
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
