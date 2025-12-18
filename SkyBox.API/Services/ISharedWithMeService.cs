using SkyBox.API.Contracts.SharedWithMe;

namespace SkyBox.API.Services;

public interface ISharedWithMeService
{
    /// <summary>
    /// Returns a unified paginated list of files and folders
    /// shared with the current user.
    /// </summary>
    Task<Result<PaginatedList<SharedWithMeItemResponse>>> GetSharedWithMeAsync(string userId,RequestFilters filters,CancellationToken cancellationToken = default);
}
