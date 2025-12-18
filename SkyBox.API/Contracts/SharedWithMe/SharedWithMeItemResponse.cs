namespace SkyBox.API.Contracts.SharedWithMe;

public record SharedWithMeItemResponse(
    Guid Id,
    string Name,
    string Type,
    string OwnerName,
    string Permission,
    DateTime SharedAt,
    string? ContentType,
    long? Size
);