namespace SkyBox.API.Contracts.Folder;

public record FolderContentResponse(
    Guid Id,
    string Name,
    bool IsFolder,
    bool IsFavorite,
    DateTime CreatedAt,
    string? ContentType,
    long? Size
);