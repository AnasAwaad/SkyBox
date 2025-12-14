using SkyBox.API.Contracts.FolderShares;

namespace SkyBox.API.Services;

public interface IFolderShareService
{
    Task<Result> ShareAsync(Guid folderId, string ownerId, ShareFolderRequest request,CancellationToken cancellationToken = default);
    Task<Result> RevokeAsync(Guid folderId, string ownerId, string sharedWithUserId,CancellationToken cancellationToken = default);
}

