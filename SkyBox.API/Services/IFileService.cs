
using SkyBox.API.Contracts.Files;

namespace SkyBox.API.Services;

public interface IFileService
{
    /// <summary>
    /// Uploads a single file for the specified user.
    /// If a file with the same name already exists in the target folder,
    /// a new file version is created instead of overwriting the file.
    /// </summary>
    Task<Result<FileUploadResponse>> UploadAsync(IFormFile file,string userId, Guid? folderId = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Uploads multiple files in a single request.
    /// Each file is validated individually and versioned if it already exists.
    /// </summary>
    Task<Result<IEnumerable<FileUploadResponse>>> UploadManyAsync(IFormFileCollection files,string userId, Guid? folderId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file.
    /// The user must be the file owner or have access through sharing.
    /// </summary>
    Task<Result<FileContentDto>>DownloadAsync(Guid fileId,string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams a file with range support.
    /// Used mainly for media files such as video or audio.
    /// </summary>
    Task<Result<StreamContentDto>>StreamAsync(Guid fileId,string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a soft delete on a file.
    /// Only the file owner is allowed to delete the file.
    /// </summary>
    Task<Result> DeleteAsync(Guid fileId,string userId, CancellationToken cancellationToken = default);
}
