using SkyBox.API.Contracts.Folder;
using SkyBox.API.Contracts.SharedWithMe;
using System.Linq.Dynamic.Core;


namespace SkyBox.API.Services;

public class SharedWithMeService(ApplicationDbContext dbContext) : ISharedWithMeService
{
    public async Task<Result<PaginatedList<SharedWithMeItemResponse>>> GetSharedWithMeAsync(string userId, RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var fileSharedQuery = dbContext.FileShares
            .AsNoTracking()
            .Where(x => x.SharedWithUserId == userId && !x.IsRevoked)
            .Select(x=>new
            {
                Id = x.FileId,
                Name = x.File.FileName,
                Type = "File",
                Permission = x.Permission,
                OwnerName = x.Owner.UserName!,
                SharedAt = x.CreatedAt,
                ContentType = (string?)x.File.ContentType,
                Size = (long?)x.File.Size
            });


        var folderSharedQuery = dbContext.FolderShares
            .AsNoTracking()
            .Where(x => x.SharedWithUserId == userId && !x.IsRevoked)
            .Select(x => new
            {
                Id = x.FolderId,
                x.Folder.Name,
                Type = "Folder",
                Permission = x.Permission,
                OwnerName = x.Owner.UserName!,
                SharedAt = x.CreatedAt,
                ContentType = (string?)null,
                Size = (long?)null
            });


        var query = fileSharedQuery.Concat(folderSharedQuery);

        if (!string.IsNullOrEmpty(filters.SearchValue))
            query = query.Where(x => x.Name.ToLower().Contains(filters.SearchValue.ToLower()));


        if (!string.IsNullOrEmpty(filters.SortColumn))
            query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");

        var source = query
            .ProjectToType<SharedWithMeItemResponse>()
            .AsNoTracking();

        var result = await PaginatedList<SharedWithMeItemResponse>.CreateAsync(source, filters.PageNumber, filters.PageSize, cancellationToken);

        return Result.Success(result);
    }
}
