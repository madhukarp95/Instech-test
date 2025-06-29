using Claims.Models;
using Claims.Models.DTO;

namespace Claims.Services.Claims;

public interface IClaimsService
{
    /// <summary>
    /// Adds a new claim item asynchronously.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task<Claim> AddItemAsync(ClaimDto item);

    /// <summary>
    /// Deletes a claim item asynchronously based on the provided identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteItemAsync(string id);

    /// <summary>
    /// Gets a claim item asynchronously based on the provided identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Claim?> GetClaimAsync(string id);

    /// <summary>
    /// Gets all claim items asynchronously.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<Claim>> GetClaimsAsync();
}