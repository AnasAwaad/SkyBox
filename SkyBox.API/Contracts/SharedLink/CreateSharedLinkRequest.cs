namespace SkyBox.API.Contracts.SharedLink;

public record CreateSharedLinkRequest(
    DateTime? ExpiresAt,
    int? MaxDownloads,
    string? Password,
    string Permission = "view"
);