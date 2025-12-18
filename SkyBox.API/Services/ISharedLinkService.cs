using SkyBox.API.Contracts.Files;
using SkyBox.API.Contracts.SharedLink;

namespace SkyBox.API.Services;

public interface ISharedLinkService
{
    /// <summary>
    /// Get paginated shared links created by the user.
    /// </summary>
    Task<Result<PaginatedList<SharedLinkResponse>>> GetMyLinksAsync(string userId, RequestFilters filters, CancellationToken cancellationToken);
    
    /// <summary>
    /// Create a new shared link for a file.
    /// </summary>
    Task<Result<SharedLinkResponse>> CreateSharedLinkAsync(Guid fileId,string ownerId, CreateSharedLinkRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a file using a shared link token.
    /// </summary>
    Task<Result<FileContentDto>> DownloadByTokenAsync(string token,string? password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream a file using a shared link token.
    /// </summary>
    Task<Result<StreamContentDto>> StreamByTokenAsync(string token,string? password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get public metadata for a shared link.
    /// </summary>
    Task<Result<SharedLinkPublicInfoResponse>> GetInfoByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a shared link owned by the user.
    /// </summary>
    Task<Result> DeleteAsync(Guid sharedLinkId, string ownerId, CancellationToken cancellationToken = default);
}
