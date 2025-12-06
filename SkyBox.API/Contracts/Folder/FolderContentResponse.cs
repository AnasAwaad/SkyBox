namespace SkyBox.API.Contracts.Folder;

public record FolderContentResponse(
    IEnumerable<FolderChildrenResponse> Folders,
    IEnumerable<FolderFileChildrenResponse> Files
);
