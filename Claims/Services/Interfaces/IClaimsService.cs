using Claims.Models;

namespace Claims.Services.Interfaces
{
    public interface IClaimsService
    {
        Task AddItemAsync(Claim item);
        Task DeleteItemAsync(string id);
        Task<Claim?> GetClaimAsync(string id);
        Task<IEnumerable<Claim>> GetClaimsAsync();
    }
}