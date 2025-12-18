using SkyBox.API.Contracts.FolderShares;

namespace SkyBox.API.Services;

public interface IFolderShareService
{
    /// <summary>
    /// Share a folder with another user.
    /// </summary>
    Task<Result> ShareAsync(Guid folderId, string ownerId, ShareFolderRequest request,CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke folder access from a user.
    /// </summary>
    Task<Result> RevokeAsync(Guid folderId, string ownerId, string sharedWithUserId,CancellationToken cancellationToken = default);
}

