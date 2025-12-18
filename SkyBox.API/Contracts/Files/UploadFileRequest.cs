namespace SkyBox.API.Contracts.Files;

public record UploadFileRequest(
    IFormFile File,
    Guid? FolderId = null
);
