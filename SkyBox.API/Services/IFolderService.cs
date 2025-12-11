using SkyBox.API.Contracts.Folder;

namespace SkyBox.API.Services;

public interface IFolderService
{
    Task<Result<PaginatedList<FolderContentResponse>>> GetFolderContentAsync(RequestFilters filters, Guid folderId,string userId,CancellationToken ct = default);
    Task<Result<FolderResponse>> CreateFolderAsync(FolderRequest request,string userId, CancellationToken cancellationToken);
    Task<Result> RenameFolderAsync(Guid folderId, string newName, string userId, CancellationToken cancellationToken);
}
