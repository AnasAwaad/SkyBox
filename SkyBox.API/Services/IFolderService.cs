using SkyBox.API.Contracts.Folder;

namespace SkyBox.API.Services;

public interface IFolderService
{
    Task<Result<PaginatedList<FolderContentResponse>>> GetFolderContentAsync(Guid? folderId,RequestFilters filters,CancellationToken ct = default);
    Task<Result> CreateFolderAsync(FolderRequest request, CancellationToken cancellationToken);
    Task<Result> RenameFolderAsync(Guid folderId, string newName, CancellationToken cancellationToken);
}
