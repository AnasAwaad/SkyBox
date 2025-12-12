using SkyBox.API.Contracts.FileVersions;

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

    /// <summary>
    /// Restore a specific version as the latest.
    /// </summary>
    Task<Result> RestoreVersionAsync(Guid fileId, Guid versionId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a specific version of a file.
    /// </summary>
    Task<Result<FileContentDto>> DownloadVersionAsync(Guid fileId, Guid versionId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a specific version of a file.
    /// </summary>
    Task<Result> DeleteVersionAsync(Guid fileId, Guid versionId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update the description of a specific version of a file.
    /// </summary>
    Task<Result> UpdateVersionDescriptionAsync(Guid fileId, Guid versionId, string userId, string description, CancellationToken cancellationToken = default);

}
