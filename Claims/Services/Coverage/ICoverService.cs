using Claims.Models.Cover;
using Claims.Models.DTO;

namespace Claims.Services.Coverage;

public interface ICoverService
{
    /// <summary>
    /// Adds a new cover item asynchronously.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task<Cover> AddItemAsync(CoverDto item);

    /// <summary>
    /// Deletes a cover item asynchronously based on the provided identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteItemAsync(string id);

    /// <summary>
    /// Gets a cover item asynchronously based on the provided identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Cover?> GetCoverAsync(string id);

    /// <summary>
    /// Gets all cover items asynchronously.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<Cover>> GetCoverAsync();

    /// <summary>
    /// Computes the premium for a given cover based on the provided cover data.
    /// </summary>
    /// <param name="coverDto"></param>
    /// <returns></returns>
    decimal ComputePremium(CoverDto coverDto);
}
