namespace SkyBox.API.Contracts.Folder;

public record FolderFileChildrenResponse(
    Guid Id,
    string Name,
    bool IsFavorite,
    DateTime CreatedAt,
    string ContentType,
    long Size
);