
namespace SkyBox.API.Services;

public interface IFileService
{
    Task<Result<PaginatedList<FileListItemResponse>>> GetFilesAsync(RequestFilters filters,string userId,CancellationToken cancellationToken = default);
    Task<Result<FileUploadResponse>> UploadAsync(IFormFile file,string userId, Guid? folderId = null, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<FileUploadResponse>>> UploadManyAsync(IFormFileCollection files,string userId, Guid? folderId = null, CancellationToken cancellationToken = default);
    Task<Result<FileContentDto>>DownloadAsync(Guid fileId,string userId, CancellationToken cancellationToken = default);
    Task<Result<StreamContentDto>>StreamAsync(Guid fileId,string userId, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid fileId,string userId, CancellationToken cancellationToken = default);
}
