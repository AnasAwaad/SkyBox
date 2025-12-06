namespace SkyBox.API.Contracts.Folder;

public record FolderRequest(
    string Name,
    Guid? ParentFolderId
);