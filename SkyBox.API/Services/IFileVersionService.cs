using SkyBox.API.Contracts.FileVersion;

namespace SkyBox.API.Services;

public interface IFileVersionService
{
    /// <summary>
    /// List versions for a file (owner-only). Returns latest versions descending.
    /// </summary>
    Task<Result<IEnumerable<FileVersionResponse>>> GetAllVersionsAsync(Guid fileId,string currentUserId,CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new version when uploading a new file (if file exists)
    /// If file doesn't exist, create new UploadedFile (caller can use SaveNewFileAsync instead).</summary>
    /// </summary>
    Task<Result<UploadedFile>> SaveNewVersionAsync(UploadedFile existingFile,IFormFile newFile,string userId,CancellationToken cancellationToken = default);
}
