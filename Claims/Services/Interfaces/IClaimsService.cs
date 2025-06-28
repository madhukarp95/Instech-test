using Claims.Models;
using Claims.Models.DTO;

namespace Claims.Services.Interfaces
{
    public interface IClaimsService
    {
        Task<Claim> AddItemAsync(ClaimDto item);
        Task DeleteItemAsync(string id);
        Task<Claim?> GetClaimAsync(string id);
        Task<IEnumerable<Claim>> GetClaimsAsync();
    }
}