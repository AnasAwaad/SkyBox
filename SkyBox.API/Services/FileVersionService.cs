using Microsoft.AspNetCore.Identity;
using SkyBox.API.Contracts.Files;
using SkyBox.API.Contracts.FileVersions;

namespace SkyBox.API.Services;

public class FileVersionService(ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    IBlobService blobService) : IFileVersionService
{
    public async Task<Result<IEnumerable<FileVersionResponse>>> GetAllVersionsAsync(Guid fileId, string currentUserId, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files
            .Include(x => x.Versions)
            .FirstOrDefaultAsync(x => x.Id == fileId && x.OwnerId == currentUserId, cancellationToken);

        if (file is null)
            return Result.Failure<IEnumerable<FileVersionResponse>>(FileErrors.FileNotFound);

        var versions = file.Versions
            .OrderByDescending(x => x.CreatedAt)
            .Where(x=> !x.IsDeleted)
            .Adapt<IEnumerable<FileVersionResponse>>();


        return Result.Success(versions);
    }

    public async Task<Result> RestoreVersionAsync(Guid fileId, Guid versionId, string userId, CancellationToken cancellationToken = default)
    {
        var result = await ValidateAndGetFileAndVersionAsync(fileId, versionId, userId, cancellationToken);

        var (file, version) = result.Value;

        // Backup current as a version
        var currentBackup = new FileVersion
        {
            Id = Guid.NewGuid(),
            FileId = file.Id,
            FileName = file.FileName,
            StoredFileName = file.StoredFileName,
            Size = file.Size,
            ContentType = file.ContentType,
            CreatedAt = DateTime.UtcNow,
            Description = $"Backup before restore at {DateTime.UtcNow:O}"
        };
        await dbContext.FileVersions.AddAsync(currentBackup, cancellationToken);

        // Set file current pointer to version's storage path
        file.FileName = version.FileName;
        file.StoredFileName = version.StoredFileName;
        file.Size = version.Size;
        file.ContentType = version.ContentType;
        dbContext.Files.Update(file);


        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
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

        var stream = file.OpenReadStream();
        var storedFileName = await blobService.UploadAsync(stream,file.ContentType, cancellationToken);

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


    public async Task<Result<FileContentDto>> DownloadVersionAsync(Guid fileId, Guid versionId, string userId, CancellationToken cancellationToken = default)
    {
        var result = await ValidateAndGetFileAndVersionAsync(fileId, versionId, userId, cancellationToken);

        if (result.IsFailure)
            return Result.Failure<FileContentDto>(result.Error);

        var (file,version) = result.Value;

        var fileResponse = await blobService.DownloadAsync(version.StoredFileName, cancellationToken);

        return Result.Success(new FileContentDto
        {
            Content = fileResponse.Stream,
            FileName = file.FileName,
            ContentType = version.ContentType
        });
    }

    public async Task<Result> DeleteVersionAsync(Guid fileId, Guid versionId, string userId, CancellationToken cancellationToken = default)
    {
        var result = await ValidateAndGetFileAndVersionAsync(fileId, versionId, userId, cancellationToken);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        var version = result.Value.Version;

        if (version.IsDeleted)
            return Result.Failure(FileErrors.VersionAlreadyDeleted);

        version.IsDeleted = true;
        version.DeletedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdateVersionDescriptionAsync(Guid fileId, Guid versionId, string userId, string description, CancellationToken cancellationToken = default)
    {
        var result = await ValidateAndGetFileAndVersionAsync(fileId, versionId, userId, cancellationToken);

        if (result.IsFailure)
            return Result.Failure(result.Error);
        
        var version = result.Value.Version;

        version.Description = description;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }


    /// <summary>
    /// Validates that the file and version exist and belong to the user.
    /// Returns the file and version if valid.
    /// </summary>
    private async Task<Result<FileAndVersionResult>> ValidateAndGetFileAndVersionAsync(Guid fileId, Guid versionId, string userId, CancellationToken cancellationToken)
    {
        var file = await dbContext.Files
            .Include(f => f.Versions)
            .FirstOrDefaultAsync(f => f.Id == fileId && f.DeletedAt == null, cancellationToken);

        if (file is null)
            return Result.Failure<FileAndVersionResult>(FileErrors.FileNotFound);

        if (file.OwnerId != userId)
            return Result.Failure<FileAndVersionResult>(FileErrors.Forbidden);

        var version = file.Versions.FirstOrDefault(v => v.Id == versionId && !v.IsDeleted);

        if (version is null)
            return Result.Failure<FileAndVersionResult>(FileErrors.FileNotFound);

        return Result.Success(new FileAndVersionResult(file,version));
    }
}
