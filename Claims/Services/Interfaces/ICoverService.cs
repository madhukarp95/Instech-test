using Claims.Models;
using Claims.Models.DTO;

namespace Claims.Services.Interfaces
{
    public interface ICoverService
    {
        Task<Cover> AddItemAsync(CoverDto item);
        Task DeleteItemAsync(string id);
        Task<Cover?> GetCoverAsync(string id);
        Task<IEnumerable<Cover>> GetCoverAsync();
    }
}
