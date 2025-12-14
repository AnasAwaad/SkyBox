using SkyBox.API.Contracts.FileShares;

namespace SkyBox.API.Services;

public interface IFileShareService
{
    Task<Result> ShareAsync(Guid fileId, string ownerId, ShareFileRequest request,CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<SharedWithMeResponse>>> GetSharedWithMeAsync(string userId,int pageNumber,int pageSize,CancellationToken cancellationToken = default);
    Task<Result> RevokeAsync(Guid fileId, string ownerId, string sharedWithUserId, CancellationToken cancellationToken = default);
    Task<Result> UpdatePermissionAsync(Guid fileId, string ownerId, string sharedWithUserId, SharePermission permission, CancellationToken cancellationToken = default);
}
