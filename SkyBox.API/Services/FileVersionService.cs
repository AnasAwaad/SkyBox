using Microsoft.AspNetCore.Identity;
using SkyBox.API.Contracts.FileVersion;
using SkyBox.API.Entities;

namespace SkyBox.API.Services;

public class FileVersionService(ApplicationDbContext dbContext,UserManager<ApplicationUser> userManager,IStorageQuotaService storageQuotaService) : IFileVersionService
{
    public async Task<Result<IEnumerable<FileVersionResponse>>> GetAllVersionsAsync(Guid fileId, string currentUserId, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files
            .Include(x=>x.Versions)
            .FirstOrDefaultAsync(x => x.Id == fileId && x.OwnerId == currentUserId,cancellationToken);

        if (file is null)
            return Result.Failure<IEnumerable<FileVersionResponse>>(FileErrors.FileNotFound);

        var versions = file.Versions
            .OrderByDescending(x => x.CreatedAt)
            .Adapt<IEnumerable<FileVersionResponse>>();


        return Result.Success(versions);
    }

    public async Task<Result<UploadedFile>> SaveNewVersionAsync(UploadedFile existingFile, IFormFile file, string userId, CancellationToken cancellationToken = default)
    {
        if (existingFile.OwnerId != userId)
            return Result.Failure<UploadedFile>(FileErrors.FileNotFound);

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure<UploadedFile>(FileErrors.FileNotFound);

        if (user.SubscriptionPlan != SubscriptionPlan.Business)
            return Result.Failure<UploadedFile>(FileErrors.VersioningNotAllowed);

        var storedFileName = await storageQuotaService.UploadFileAsync(file, cancellationToken);

        // create version record from existing current file
        var version = existingFile.Adapt<FileVersion>();
        version.Id = Guid.NewGuid();

        await dbContext.AddAsync(version, cancellationToken);

        // update UploadedFile to point to new current path
        existingFile.FileExtension = Path.GetExtension(file.FileName);
        existingFile.FileName = file.FileName;
        existingFile.StoredFileName = storedFileName;
        existingFile.Size = file.Length;
        existingFile.ContentType = file.ContentType;

        dbContext.Files.Update(existingFile);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(existingFile);
    }
}
