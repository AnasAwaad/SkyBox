using SkyBox.API.Contracts.Favorites;
using System.Linq.Dynamic.Core;

namespace SkyBox.API.Services;

public class FavoriteService(ApplicationDbContext dbContext) : IFavoriteService
{
    public async Task<Result<PaginatedList<FavoriteItemResponse>>> GetFavoritesAsync(string userId, RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var fileSharedQuery = dbContext.Files
            .AsNoTracking()
            .Where(x => x.OwnerId == userId && x.DeletedAt == null && x.IsFavorite)
            .Select(x => new
            {
                Id = x.Id,
                Name = x.FileName,
                Type = "File",
                ContentType = (string?)x.ContentType,
                Size = (long?)x.Size
            });


        var folderSharedQuery = dbContext.Folders
            .AsNoTracking()
            .Where(x => x.OwnerId == userId && x.IsFavorite)
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
                Type = "Folder",
                ContentType = (string?)null,
                Size = (long?)null
            });


        var query = fileSharedQuery.Concat(folderSharedQuery);

        if (!string.IsNullOrEmpty(filters.SearchValue))
            query = query.Where(x => x.Name.ToLower().Contains(filters.SearchValue.ToLower()));


        if (!string.IsNullOrEmpty(filters.SortColumn))
            query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");

        var source = query
            .ProjectToType<FavoriteItemResponse>()
            .AsNoTracking();

        var result = await PaginatedList<FavoriteItemResponse>.CreateAsync(source, filters.PageNumber, filters.PageSize, cancellationToken);

        return Result.Success(result);
    }
}
