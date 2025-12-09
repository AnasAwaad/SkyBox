namespace SkyBox.API.Contracts.SharedLink;

public record SharedLinkPublicInfoResponse(
    string FileName,
    long FileSize,
    string ContentType,
    DateTime? ExpiresAt,
    string Permission,
    bool IsExpired
);