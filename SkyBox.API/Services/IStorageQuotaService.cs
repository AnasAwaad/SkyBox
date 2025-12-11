namespace SkyBox.API.Services;

public interface IStorageQuotaService
{
    /// <summary> Save file and return stored file name (random unique key). </summary>
    Task<string> UploadFileAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task<bool> CanUploadFileAsync(string userId, long fileSizeBytes,CancellationToken cancellationToken = default);
    Task<long> GetUsedStorageAsync(string userId,CancellationToken cancellationToken = default);
}
