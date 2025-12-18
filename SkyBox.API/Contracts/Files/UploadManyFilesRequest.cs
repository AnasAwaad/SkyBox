namespace SkyBox.API.Contracts.Files;

public record UploadManyFilesRequest(
    IFormFileCollection Files,
    Guid? FolderId = null
);