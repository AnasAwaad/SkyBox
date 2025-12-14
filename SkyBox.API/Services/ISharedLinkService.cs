using SkyBox.API.Contracts.SharedLink;

namespace SkyBox.API.Services;

public interface ISharedLinkService
{
    Task<Result<PaginatedList<SharedLinkResponse>>> GetMyLinksAsync(string userId, RequestFilters filters, CancellationToken cancellationToken);
    Task<Result<SharedLinkResponse>> CreateSharedLinkAsync(Guid fileId,string ownerId, CreateSharedLinkRequest request, CancellationToken cancellationToken = default);
    Task<Result<FileContentDto>> DownloadByTokenAsync(string token,string? password, CancellationToken cancellationToken = default);
    Task<Result<StreamContentDto>> StreamByTokenAsync(string token,string? password, CancellationToken cancellationToken = default);
    Task<Result<SharedLinkPublicInfoResponse>> GetInfoByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid sharedLinkId, string ownerId, CancellationToken cancellationToken = default);
}
