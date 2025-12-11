namespace SkyBox.API.Contracts.FileVersion;

public record FileVersionResponse(
    Guid Id,
    DateTime CreatedAt,
    string CreatedBy,
    long Size,
    string ContentType,
    string? Description
);
