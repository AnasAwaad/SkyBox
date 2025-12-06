namespace SkyBox.API.Contracts.Folder;

public record FolderChildrenResponse(
    Guid Id,
    string Name,
    bool IsFavorite,
    DateTime CreatedAt
);