using SkyBox.API.Contracts.FileShares;

namespace SkyBox.API.Services;

public interface IFileShareService
{
    /// <summary>
    /// Shares a file with another user and assigns a permission level.
    /// Only the file owner is allowed to perform this action.
    /// </summary>
    Task<Result> ShareAsync(Guid fileId, string ownerId, ShareFileRequest request,CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of files shared with the given user.
    /// </summary>
    Task<Result<PaginatedList<SharedWithMeResponse>>> GetSharedWithMeAsync(string userId,int pageNumber,int pageSize,CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes access to a shared file for a specific user.
    /// Only the file owner can revoke sharing.
    /// </summary>
    Task<Result> RevokeAsync(Guid fileId, string ownerId, string sharedWithUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the permission level of a shared file for a specific user.
    /// Only the file owner can modify permissions.
    /// </summary>
    Task<Result> UpdatePermissionAsync(Guid fileId, string ownerId, string sharedWithUserId, SharePermission permission, CancellationToken cancellationToken = default);
}
