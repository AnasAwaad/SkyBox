using Microsoft.EntityFrameworkCore;
using SkyBox.API.Contracts.FileShares;

namespace SkyBox.API.Services;

public class FileShareService(ApplicationDbContext dbContext) : IFileShareService
{
    public async Task<Result<IEnumerable<SharedWithMeResponse>>> GetSharedWithMeAsync(string userId, CancellationToken cancellationToken = default)
    {
        var shares = await dbContext.FileShares
            .Where(x => x.SharedWithUserId == userId && !x.IsRevoked)
            .Include(x => x.File)
            .Include(x => x.Owner)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var response = shares.Adapt<IEnumerable<SharedWithMeResponse>>();

        return Result.Success(response);
    }


    public async Task<Result> ShareAsync(Guid fileId, string ownerId, ShareFileRequest request, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files
            .FirstOrDefaultAsync(f => f.Id == fileId && f.OwnerId == ownerId, cancellationToken);

        if (file is null)
            return Result.Failure(FileErrors.FileNotFound);

        var shareExists = await dbContext.FileShares.AnyAsync(x =>
            x.FileId == fileId &&
            x.SharedWithUserId == request.SharedWithUserId &&
            !x.IsRevoked, cancellationToken);

        if (shareExists)
            return Result.Failure(FileShareErrors.AlreadyShared);

        var share = new FileShare
        {
            FileId = fileId,
            OwnerId = ownerId,
            SharedWithUserId = request.SharedWithUserId,
            Permission = request.Permission
        };

        dbContext.FileShares.Add(share);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RevokeAsync(Guid fileId, string ownerId, string sharedWithUserId, CancellationToken cancellationToken = default)
    {
        var share = await dbContext.FileShares
            .FirstOrDefaultAsync(x => x.FileId == fileId && x.OwnerId == ownerId && x.SharedWithUserId == sharedWithUserId &&!x.IsRevoked,
            cancellationToken);

        if (share is null)
            return Result.Failure(FileShareErrors.ShareNotFound);

        share.IsRevoked = true;
        await dbContext.SaveChangesAsync();

        return Result.Success();
    }


}
