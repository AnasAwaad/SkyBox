namespace SkyBox.API.Services;

public interface IFileService
{
    Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task<IEnumerable<Guid>> UploadManyAsync(IFormFileCollection files, CancellationToken cancellationToken = default);
    Task<(byte[] fileContent,string contentType,string fileName)>DownloadAsync(Guid fileId, CancellationToken cancellationToken = default);
    Task<(FileStream? stream,string contentType,string fileName)>StreamAsync(Guid fileId, CancellationToken cancellationToken = default);
}
