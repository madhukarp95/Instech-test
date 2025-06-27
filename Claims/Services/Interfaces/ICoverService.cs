using Claims.Models;

namespace Claims.Services.Interfaces
{
    public interface ICoverService
    {
        Task AddItemAsync(Cover item);
        Task DeleteItemAsync(string id);
        Task<Cover?> GetCoverAsync(string id);
        Task<IEnumerable<Cover>> GetCoverAsync();
    }
}
