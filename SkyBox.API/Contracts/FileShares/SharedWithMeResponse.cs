namespace SkyBox.API.Contracts.FileShares;

public record SharedWithMeResponse(
    Guid FileId,
    string FileName,
    long Size,
    string ContentType,
    string OwnerName,
    SharePermission Permission,
    DateTime SharedAt
);
