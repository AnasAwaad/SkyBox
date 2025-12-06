namespace SkyBox.API.Contracts;

public record UploadFileRequest(
    IFormFile File,
    Guid? FolderId = null
);
