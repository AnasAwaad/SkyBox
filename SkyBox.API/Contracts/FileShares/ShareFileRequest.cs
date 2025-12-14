namespace SkyBox.API.Contracts.FileShares;

public record ShareFileRequest(
    string SharedWithUserId,
    SharePermission Permission
);
