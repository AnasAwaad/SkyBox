namespace SkyBox.API.Contracts.Favorites;

public record FavoriteItemResponse(
    Guid Id,
    string Name,
    string Type,
    DateTime CreatedAt,
    string? ContentType,
    long? Size
);