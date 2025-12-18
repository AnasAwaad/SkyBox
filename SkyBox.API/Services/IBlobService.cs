using SkyBox.API.Contracts.Files;

namespace SkyBox.API.Services;

public interface IBlobService
{
    Task<string> UploadAsync(Stream stream , string contentType , CancellationToken cancellationToken = default);
    Task<FileResponse> DownloadAsync(string fileName,CancellationToken cancellationToken = default);
    Task DeleteAsync(string fileName,CancellationToken cancellationToken= default);
}