namespace SkyBox.API.Contracts.Folder;

public record FolderResponse(
    Guid Id,
    string Name,
    string OwnerId,
    Guid? ParentId,
    DateTime CreatedAt
);
