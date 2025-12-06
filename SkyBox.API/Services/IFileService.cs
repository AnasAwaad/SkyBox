
namespace SkyBox.API.Services;

public interface IFileService
{
    Task<Result<PaginatedList<FileListItemResponse>>> GetFilesAsync(RequestFilters filters,CancellationToken cancellationToken = default);
    Task<Result<FileUploadResponse>> UploadAsync(IFormFile file, Guid? folderId = null, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<FileUploadResponse>>> UploadManyAsync(IFormFileCollection files, Guid? folderId = null, CancellationToken cancellationToken = default);
    Task<Result<FileContentDto>>DownloadAsync(Guid fileId, CancellationToken cancellationToken = default);
    Task<Result<StreamContentDto>>StreamAsync(Guid fileId, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid fileId, CancellationToken cancellationToken = default);
}
