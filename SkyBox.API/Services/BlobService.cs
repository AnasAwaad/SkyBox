using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SkyBox.API.Contracts.Files;
using System.Net.Mime;

namespace SkyBox.API.Services;
public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private const string ContainerName = "files";

    public BlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<string> UploadAsync(Stream stream,string contentType,CancellationToken cancellationToken = default)
    {
        var containerClient =
            _blobServiceClient.GetBlobContainerClient(ContainerName);

        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var fileName = Guid.NewGuid().ToString();

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(
            stream,
            new BlobHttpHeaders { ContentType = contentType },
            cancellationToken: cancellationToken);

        return fileName;
    }

    public async Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var containerClient =
            _blobServiceClient.GetBlobContainerClient(ContainerName);

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<FileResponse> DownloadAsync(string fileName,CancellationToken cancellationToken = default)
    {
        var containerClient =
            _blobServiceClient.GetBlobContainerClient(ContainerName);

        var blobClient = containerClient.GetBlobClient(fileName);

        var properties = await blobClient.GetPropertiesAsync(cancellationToken:cancellationToken);

        var stream = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);

        return new FileResponse(
            stream,
            properties.Value.ContentType ?? MediaTypeNames.Application.Octet
        );
    }
}
