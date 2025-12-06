
namespace SkyBox.API.Services;

public interface IFileService
{
    Task<Result<PaginatedList<FileListItemResponse>>> GetFilesAsync(RequestFilters filters,CancellationToken cancellationToken = default);
    Task<FileUploadResponse> UploadAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task<IEnumerable<FileUploadResponse>> UploadManyAsync(IFormFileCollection files, CancellationToken cancellationToken = default);
    Task<Result<FileContentDto>>DownloadAsync(Guid fileId, CancellationToken cancellationToken = default);
    Task<Result<StreamContentDto>>StreamAsync(Guid fileId, CancellationToken cancellationToken = default);
}
