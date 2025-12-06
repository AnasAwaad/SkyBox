using SkyBox.API.Contracts.Folder;
using SkyBox.API.Persistence;
using System.Threading;
using System.Linq.Dynamic.Core;


namespace SkyBox.API.Services;

public class FolderService(ApplicationDbContext dbContext) : IFolderService
{
    public async Task<Result> CreateFolderAsync(FolderRequest request, CancellationToken cancellationToken)
    {

        Guid? parentId = null;

        if (!string.IsNullOrEmpty(request.ParentFolderId.ToString()))
        {
            var parentFolder = await dbContext.Folders.FindAsync(request.ParentFolderId);

            if (parentFolder is null)
                return Result.Failure(FolderErrors.ParentFolderNotFound);

            parentId = parentFolder.Id;

        }

        var folder = new Folder
        {
            Name = request.Name,
            ParentId = parentId,
        };

        await dbContext.Folders.AddAsync(folder,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<PaginatedList<FolderContentResponse>>> GetFolderContentAsync(Guid? folderId, RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var folder = await dbContext.Folders.FindAsync(folderId);

        if(folder is null)
            return Result.Failure<PaginatedList<FolderContentResponse>>(FolderErrors.FolderNotFound);

        //var folderQuery = dbContext.Folders
        //    .AsNoTracking()
        //    .Where(f => f.ParentId == folderId)
        //    .Select(f => new FolderContentResponse(f.Id,f.Name,f.IsFavorite,f.CreatedAt,null,null));

        //var query = dbContext.Files
        //    .AsNoTracking()
        //    .Where(f => f.FolderId == folderId && f.DeletedAt == null)
        //    .Select(f => new FolderContentResponse(f.Id, f.FileName, f.IsFavorite, f.UploadedAt, f.Size, f.ContentType))
        //    .Concat(folderQuery);

        var query = dbContext.Folders
           .Include(x => x.Files)
           .Include(x => x.Folders)
           .Where(x => x.Id == folderId);


        if (!string.IsNullOrEmpty(filters.SearchValue))
            query = query.Where(x => x.Name.Contains(filters.SearchValue.ToLower()));


        if (!string.IsNullOrEmpty(filters.SortColumn))
            query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");

        var source = query
            .ProjectToType<FolderContentResponse>()
            .AsNoTracking();

        var result = await PaginatedList<FolderContentResponse>.CreateAsync(source, filters.PageNumber, filters.PageSize, cancellationToken);

        return Result.Success(result);

    }

    public async Task<Result> RenameFolderAsync(Guid folderId, string newName, CancellationToken cancellationToken)
    {
        var folder = await dbContext.Folders.FindAsync(folderId);

        if(folder is null)
            return Result.Failure(FolderErrors.FolderNotFound);

        folder.Name = newName;
        folder.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return Result.Success();
    }
}
