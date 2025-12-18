using SkyBox.API.Contracts.Favorites;

namespace SkyBox.API.Services;

public interface IFavoriteService
{
    /// <summary>
    /// Returns a unified list of user's favorite files and folders.
    /// </summary>
    Task<Result<PaginatedList<FavoriteItemResponse>>> GetFavoritesAsync(string userId,RequestFilters filters,CancellationToken cancellationToken = default);
}
