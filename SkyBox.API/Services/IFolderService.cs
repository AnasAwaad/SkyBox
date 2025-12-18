using SkyBox.API.Contracts.Folder;

namespace SkyBox.API.Services;

public interface IFolderService
{
    Task<Result<PaginatedList<FolderContentResponse>>> GetFolderRootContentAsync(RequestFilters filters, string userId, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<FolderContentResponse>>> GetFolderContentAsync(RequestFilters filters, Guid? folderId,string userId,CancellationToken ct = default);
    Task<Result<FolderResponse>> CreateFolderAsync(FolderRequest request,string userId, CancellationToken cancellationToken);
    Task<Result> RenameFolderAsync(Guid folderId, string newName, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Performs a toggle favorite status on folder.
    /// Only the folder owner is allowed to toggle status on file.
    /// </summary>
    Task<Result> ToggleFavoriteStatusAsync(Guid folderId, string userId, CancellationToken cancellationToken = default);
}
