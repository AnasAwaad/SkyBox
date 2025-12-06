namespace SkyBox.API.Services;

public interface ITrashService
{
    Task<Result<PaginatedList<TrashedFileResponse>>> GetTrashFilesAsync(RequestFilters filters, CancellationToken cancellationToken = default);
    Task<Result> RestoreAsync(Guid fileId, CancellationToken cancellationToken = default);
    Task<Result> PermanentlyDeleteAsync(Guid fileId, CancellationToken cancellationToken = default);
    Task<Result<int>> EmptyTrashAsync(CancellationToken cancellationToken = default);
    Task<int> PermanentlyDeleteExpiredAsync();

}
