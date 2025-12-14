namespace SkyBox.API.Contracts.FolderShares;

public record ShareFolderRequest(
    string SharedWithUserId,
    SharePermission Permission
);
