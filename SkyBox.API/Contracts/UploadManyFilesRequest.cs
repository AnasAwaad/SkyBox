namespace SkyBox.API.Contracts;

public record UploadManyFilesRequest(
    IFormFileCollection Files
);