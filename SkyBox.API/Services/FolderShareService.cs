using SkyBox.API.Contracts.FolderShares;

namespace SkyBox.API.Services;

public class FolderShareService(ApplicationDbContext dbContext) : IFolderShareService
{
    public async Task<Result> RevokeAsync(Guid folderId, string ownerId, string sharedWithUserId, CancellationToken cancellationToken = default)
    {
        var share = await dbContext.FolderShares.FirstOrDefaultAsync(x =>
            x.FolderId == folderId &&
            x.OwnerId == ownerId &&
            x.SharedWithUserId == sharedWithUserId &&
            !x.IsRevoked,
            cancellationToken);

        if (share is null)
            return Result.Failure(FileShareErrors.ShareNotFound);

        share.IsRevoked = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ShareAsync(Guid folderId, string ownerId, ShareFolderRequest request, CancellationToken cancellationToken = default)
    {
        var folder = await dbContext.Folders
            .FirstOrDefaultAsync(x => x.Id == folderId && x.OwnerId == ownerId, cancellationToken);

        if (folder is null)
            return Result.Failure(FolderErrors.FolderNotFound);

        var exists = await dbContext.FolderShares.AnyAsync(x =>
            x.FolderId == folderId &&
            x.SharedWithUserId == request.SharedWithUserId &&
            !x.IsRevoked,
            cancellationToken);

        if (exists)
            return Result.Failure(FileShareErrors.AlreadyShared);

        var share = new FolderShare
        {
            FolderId = folderId,
            OwnerId = ownerId,
            SharedWithUserId = request.SharedWithUserId,
            Permission = request.Permission
        };

        dbContext.FolderShares.Add(share);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

