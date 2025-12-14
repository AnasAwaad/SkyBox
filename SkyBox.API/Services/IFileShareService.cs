using SkyBox.API.Contracts.FileShares;

namespace SkyBox.API.Services;

public interface IFileShareService
{
    Task<Result> ShareAsync(Guid fileId, string ownerId, ShareFileRequest request,CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<SharedWithMeResponse>>> GetSharedWithMeAsync(string userId,CancellationToken cancellationToken = default);
    Task<Result> RevokeAsync(Guid fileId, string ownerId, string sharedWithUserId, CancellationToken cancellationToken = default);
}
