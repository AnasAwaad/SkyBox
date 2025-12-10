namespace SkyBox.API.Services;

public interface IStorageQuotaService
{
    Task<bool> CanUploadFileAsync(string userId, long fileSizeBytes,CancellationToken cancellationToken = default);
    Task<long> GetUsedStorageAsync(string userId,CancellationToken cancellationToken = default);
}
