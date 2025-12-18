using SkyBox.API.Contracts.Files;

namespace SkyBox.API.Services;

public interface ITrashService
{
    /// <summary>
    /// Get paginated list of trashed files.
    /// </summary>
    Task<Result<PaginatedList<TrashedFileResponse>>> GetTrashFilesAsync(RequestFilters filters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Restore a soft-deleted file from trash.
    /// </summary>
    Task<Result> RestoreAsync(Guid fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently delete a file from trash.
    /// </summary>
    Task<Result> PermanentlyDeleteAsync(Guid fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently delete all files in trash.
    /// </summary>
    Task<Result<int>> EmptyTrashAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently delete expired trashed files.
    /// Intended for background jobs.
    /// </summary>
    Task<int> PermanentlyDeleteExpiredAsync();

}
