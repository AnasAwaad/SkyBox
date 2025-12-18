using SkyBox.API.Contracts.Folder;
using System.Linq.Dynamic.Core;


namespace SkyBox.API.Services;

public class FolderService(ApplicationDbContext dbContext) : IFolderService
{
    public async Task<Result<FolderResponse>> CreateFolderAsync(FolderRequest request, string userId, CancellationToken cancellationToken)
    {
        if (request.ParentFolderId.HasValue)
        {
            var parentFolder = await dbContext.Folders.FirstOrDefaultAsync(x => x.Id == request.ParentFolderId.Value && x.OwnerId == userId);

            if (parentFolder is null)
                return Result.Failure<FolderResponse>(FolderErrors.ParentFolderNotFound);
        }

        // Check for existing folder with the same name in the same parent folder
        var existingFolder = await dbContext.Folders
            .AnyAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.ParentId == request.ParentFolderId && x.OwnerId == userId, cancellationToken);

        if (existingFolder)
            return Result.Failure<FolderResponse>(FolderErrors.FolderExists);


        var folder = new Folder
        {
            Name = request.Name,
            ParentId = request.ParentFolderId,
            OwnerId = userId,
        };

        await dbContext.Folders.AddAsync(folder, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(folder.Adapt<FolderResponse>());
    }

    public async Task<Result<PaginatedList<FolderContentResponse>>> GetFolderRootContentAsync(RequestFilters filters, string userId, CancellationToken cancellationToken = default)
    {
        var foldersQuery = dbContext.Folders
            .AsNoTracking()
            .Where(f => f.ParentId == null && f.OwnerId==userId)
            .Select(f => new
            {
                f.Id,
                f.Name,
                IsFolder = true,
                f.IsFavorite,
                f.CreatedAt,
                ContentType = (string?)null,
                Size = (long?)null
            });

        var filesQuery = dbContext.Files
            .AsNoTracking()
            .Where(f => f.FolderId == null && f.OwnerId==userId && f.DeletedAt == null)
            .Select(f => new
            {
                f.Id,
                Name = f.FileName,
                IsFolder = false,
                f.IsFavorite,
                CreatedAt = f.UploadedAt,
                ContentType = (string?)f.ContentType,
                Size = (long?)f.Size
            });


        var query = foldersQuery.Concat(filesQuery);


        if (!string.IsNullOrEmpty(filters.SearchValue))
            query = query.Where(x => x.Name.Contains(filters.SearchValue, StringComparison.CurrentCultureIgnoreCase));


        if (!string.IsNullOrEmpty(filters.SortColumn))
            query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");

        var source = query
            .ProjectToType<FolderContentResponse>()
            .AsNoTracking();

        var result = await PaginatedList<FolderContentResponse>.CreateAsync(source, filters.PageNumber, filters.PageSize, cancellationToken);

        return Result.Success(result);

    }
    public async Task<Result<PaginatedList<FolderContentResponse>>> GetFolderContentAsync(RequestFilters filters, Guid? folderId, string userId, CancellationToken cancellationToken = default)
    {
        if (folderId.HasValue)
        {
            // Get root content
            var folder = await dbContext.Folders
                .FirstOrDefaultAsync(f => f.Id == folderId, cancellationToken);

            if (folder is null)
                return Result.Failure<PaginatedList<FolderContentResponse>>(FolderErrors.FolderNotFound);

            // Authorization check
            if (!await CanAccessFolderAsync(folder, userId))
                return Result.Failure<PaginatedList<FolderContentResponse>>(FolderShareErrors.PermissionDenied);
        }


        var foldersQuery = dbContext.Folders
            .AsNoTracking()
            .Where(f => f.ParentId == folderId)
            .Select(f => new
            {
                f.Id,
                Name = f.Name,
                IsFolder = true,
                f.IsFavorite,
                CreatedAt = f.CreatedAt,
                ContentType = (string?)null,
                Size = (long?)null
            });

        var filesQuery = dbContext.Files
            .AsNoTracking()
            .Where(f => f.FolderId == folderId && f.DeletedAt == null)
            .Select(f => new
            {
                f.Id,
                Name = f.FileName,
                IsFolder = false,
                f.IsFavorite,
                CreatedAt = f.UploadedAt,
                ContentType = (string?)f.ContentType,
                Size = (long?)f.Size
            });


        var query = foldersQuery.Concat(filesQuery);


        if (!string.IsNullOrEmpty(filters.SearchValue))
            query = query.Where(x => x.Name.ToLower().Contains(filters.SearchValue.ToLower()));


        if (!string.IsNullOrEmpty(filters.SortColumn))
            query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");

        var source = query
            .ProjectToType<FolderContentResponse>()
            .AsNoTracking();

        var result = await PaginatedList<FolderContentResponse>.CreateAsync(source, filters.PageNumber, filters.PageSize, cancellationToken);

        return Result.Success(result);

    }

    public async Task<Result> RenameFolderAsync(Guid folderId, string newName, string userId, CancellationToken cancellationToken)
    {
        var folder = await dbContext.Folders
            .FirstOrDefaultAsync(f => f.Id == folderId && f.OwnerId == userId, cancellationToken);

        if (folder is null)
            return Result.Failure(FolderErrors.FolderNotFound);

        folder.Name = newName;
        folder.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }


    public async Task<Result> ToggleFavoriteStatusAsync(Guid folderId, string userId, CancellationToken cancellationToken = default)
    {
        var folder = await dbContext.Folders.FirstOrDefaultAsync(x => x.Id == folderId, cancellationToken);

        if (folder is null)
            return Result.Failure(FolderErrors.FolderNotFound);

        folder.IsFavorite = !folder.IsFavorite;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<bool> CanAccessFolderAsync(Folder folder, string userId)
    {
        // Owner
        if (folder.OwnerId == userId)
            return true;

        // Direct folder share
        var hasDirectShare = await dbContext.FolderShares.AnyAsync(x =>
            x.FolderId == folder.Id &&
            x.SharedWithUserId == userId &&
            !x.IsRevoked);

        if (hasDirectShare)
            return true;

        // Inherit from parent folder
        if (folder.ParentId != null)
        {
            var hasParentShare = await dbContext.FolderShares.AnyAsync(x =>
                x.FolderId == folder.ParentId &&
                x.SharedWithUserId == userId &&
                !x.IsRevoked);

            if (hasParentShare)
                return true;
        }

        return false;
    }

}
