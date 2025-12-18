using SkyBox.API.Contracts.Files;
using SkyBox.API.Contracts.FileVersions;

namespace SkyBox.API.Services;

public interface IFileVersionService
{
    /// <summary>
    /// Retrieves all non-deleted versions of a file.
    /// Only the file owner is allowed to access version history.
    /// </summary>
    Task<Result<IEnumerable<FileVersionResponse>>> GetAllVersionsAsync(Guid fileId,string currentUserId,CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new file version when uploading a new file.
    /// Requires a subscription plan that supports versioning.
    /// </summary>
    Task<Result<UploadedFile>> SaveNewVersionAsync(UploadedFile existingFile,IFormFile newFile,string userId,CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores a specific version and makes it the current file.
    /// A backup of the current version is created automatically.
    /// </summary>
    Task<Result> RestoreVersionAsync(Guid fileId,Guid versionId,string userId,CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the content of a specific file version.
    /// </summary>
    Task<Result<FileContentDto>> DownloadVersionAsync(Guid fileId,Guid versionId,string userId,CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes a file version.
    /// </summary>
    Task<Result> DeleteVersionAsync(Guid fileId,Guid versionId,string userId,CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the description of a file version.
    /// </summary>
    Task<Result> UpdateVersionDescriptionAsync(Guid fileId,Guid versionId,string userId,string description,CancellationToken cancellationToken = default);
}
