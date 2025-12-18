using SkyBox.API.Contracts.Files;
using SkyBox.API.Contracts.FileVersions;
using System.Linq.Dynamic.Core;

namespace SkyBox.API.Services;

public class FileService(IStorageQuotaService storageQuotaService,
    IFileVersionService fileVersionService,
    ApplicationDbContext dbContext,
    IBlobService blobService) : IFileService
{
    public async Task<Result<FileUploadResponse>> UploadAsync(IFormFile file,string userId, Guid? folderId = null, CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length <= 0)
            return Result.Failure<FileUploadResponse>(FileErrors.EmptyFile);

        var folderResult = await EnsureFolderBelongsToUserAsync(folderId, userId, cancellationToken);
        if (folderResult.IsFailure)
            return Result.Failure<FileUploadResponse>(folderResult.Error);

        var canUpload = await storageQuotaService.CanUploadFileAsync(userId, file.Length, cancellationToken);
        if (!canUpload)
            return Result.Failure<FileUploadResponse>(FileErrors.StorageQuotaExceeded);

        var saveResult = await SaveOrCreateVersionAsync(file, userId, folderId, cancellationToken);
        if (saveResult.IsFailure)
            return Result.Failure<FileUploadResponse>(saveResult.Error);

        if (saveResult.Value.AlreadyPersisted)
            return Result.Success(saveResult.Value.File.Adapt<FileUploadResponse>());

        await dbContext.AddAsync(saveResult.Value.File, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(saveResult.Value.File.Adapt<FileUploadResponse>());
    }

    public async Task<Result<IEnumerable<FileUploadResponse>>> UploadManyAsync(IFormFileCollection files,string userId, Guid? folderId = null, CancellationToken cancellationToken = default)
    {
        if (files is null || files.Count == 0)
            return Result.Failure<IEnumerable<FileUploadResponse>>(FileErrors.NoFilesProvided);

        var nonEmptyFiles = files.Where(f => f is not null && f.Length > 0).ToList();
        if (!nonEmptyFiles.Any())
            return Result.Failure<IEnumerable<FileUploadResponse>>(FileErrors.EmptyFilesOnly);


        var folderResult = await EnsureFolderBelongsToUserAsync(folderId, userId, cancellationToken);
        if (folderResult.IsFailure)
            return Result.Failure<IEnumerable<FileUploadResponse>>(folderResult.Error);


        var totalSize = files.Sum(x=>x.Length);
        var canUpload = await storageQuotaService.CanUploadFileAsync(userId, totalSize, cancellationToken);

        if (!canUpload)
            return Result.Failure<IEnumerable<FileUploadResponse>>(FileErrors.StorageQuotaExceeded);

        List<UploadedFile> uploadedFiles = [];
        var responses = new List<FileUploadResponse>();

        foreach (var file in files)
        {
            var saveResult = await SaveOrCreateVersionAsync(file, userId, folderId, cancellationToken);


            if (saveResult.IsFailure)
                return Result.Failure<IEnumerable<FileUploadResponse>>(saveResult.Error);

            if (saveResult.Value.AlreadyPersisted)
            {
                responses.Add(saveResult.Value.File.Adapt<FileUploadResponse>());
                continue;
            }

            uploadedFiles.Add(saveResult.Value.File);
            responses.Add(saveResult.Value.File.Adapt<FileUploadResponse>());
        }

        if (uploadedFiles.Any())
        {
            await dbContext.AddRangeAsync(uploadedFiles, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(responses.AsEnumerable());
    }


    public async Task<Result<FileContentDto>> DownloadAsync(Guid fileId,string userId, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files
            .FirstOrDefaultAsync(f => f.Id == fileId, cancellationToken);

        if (file == null || file.DeletedAt != null)
            return Result.Failure<FileContentDto>(FileErrors.FileNotFound);

        // Authorization check
        if (!await CanAccessFileAsync(file, userId))
            return Result.Failure<FileContentDto>(FileShareErrors.PermissionDenied);


        var fileResponse = await blobService.DownloadAsync(file.StoredFileName, cancellationToken);

        var result = new FileContentDto
        {
            Content = fileResponse.Stream,
            ContentType = file.ContentType,
            FileName = file.FileName
        };

        return Result.Success(result);

    }

    public async Task<Result<StreamContentDto>> StreamAsync(Guid fileId,string userId, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files
            .FirstOrDefaultAsync(f => f.Id == fileId, cancellationToken);

        if (file == null || file.DeletedAt != null)
            return Result.Failure<StreamContentDto>(FileErrors.FileNotFound);

        // Authorization check
        if (!await CanAccessFileAsync(file, userId))
            return Result.Failure<StreamContentDto>(FileShareErrors.PermissionDenied);

        var fileResponse = await blobService.DownloadAsync(file.StoredFileName, cancellationToken);

        var result = new StreamContentDto
        {
            Stream = fileResponse.Stream,
            ContentType = fileResponse.ContentType,
            FileName = file.FileName
        };

        return Result.Success(result);
    }

    public async Task<Result> DeleteAsync(Guid fileId,string userId, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files
            .FirstOrDefaultAsync(x => x.Id == fileId && x.OwnerId == userId, cancellationToken);


        if (file == null || file.DeletedAt != null)
            return Result.Failure<StreamContentDto>(FileErrors.FileNotFound);

        // Soft delete
        file.DeletedAt = DateTime.UtcNow;
        file.IsFavorite = false;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// Validates that the folder exists and belongs to the given user.
    /// If folderId is null, returns success immediately.
    /// </summary>
    private async Task<Result> EnsureFolderBelongsToUserAsync(Guid? folderId, string userId,CancellationToken cancellationToken = default)
    {
        if(folderId is null)
            return Result.Success();

        var folder = await dbContext.Folders
                .FirstOrDefaultAsync(f => f.Id == folderId && f.OwnerId == userId, cancellationToken);

        if (folder is null)
            return Result.Failure<FileUploadResponse>(FolderErrors.FolderNotFound);

        return Result.Success();
    }


    /// <summary>
    /// Creates a new file or generates a new version if the file already exists.
    /// Returns the file and a flag indicating whether it was already persisted.
    /// </summary>
    private async Task<Result<SaveOrVersionResult>> SaveOrCreateVersionAsync(IFormFile file,string userId,Guid? folderId,CancellationToken cancellationToken = default)
    {
        var existingFile = await dbContext.Files
            .FirstOrDefaultAsync(f =>
                f.OwnerId == userId &&
                f.FolderId == folderId &&
                f.FileName == file.FileName &&
                f.DeletedAt == null,
                cancellationToken);

        if (existingFile is not null)
        {
            var versionResult = await fileVersionService.SaveNewVersionAsync(existingFile, file, userId, cancellationToken);
            if (versionResult.IsFailure)
                return Result.Failure<SaveOrVersionResult>(versionResult.Error);

            return Result.Success(new SaveOrVersionResult(versionResult.Value, true));
        }

        using Stream stream = file.OpenReadStream();
        var storedFileName = await blobService.UploadAsync(stream, file.ContentType, cancellationToken);
        //var storedFileName = await storageQuotaService.UploadFileAsync(file, cancellationToken);

        var uploadedFile = new UploadedFile
        {
            FileName = file.FileName,
            StoredFileName = storedFileName,
            ContentType = file.ContentType ,
            FileExtension = Path.GetExtension(file.FileName),
            Size = file.Length,
            UploadedAt = DateTime.UtcNow,
            FolderId = folderId,
            OwnerId = userId
        };

        return Result.Success(new SaveOrVersionResult(uploadedFile, false));
    }

    /// <summary>
    /// Determines whether the user can access the given file.
    /// Access is granted if the user is the owner, has a direct file share,
    /// or has access through a shared parent folder.
    /// </summary>
    private async Task<bool> CanAccessFileAsync(UploadedFile file, string userId)
    {
        if (file.OwnerId == userId)
            return true;

        var hasFileShare = await dbContext.FileShares
            .AnyAsync(x => x.FileId == file.Id && x.SharedWithUserId == userId && x.OwnerId == file.OwnerId && !x.IsRevoked);

        if(hasFileShare)
            return true;

        if (file.FolderId != null)
        {
            var hasFolderShare = await dbContext.FolderShares.AnyAsync(x =>
                x.FolderId == file.FolderId &&
                x.SharedWithUserId == userId &&
                x.OwnerId == file.OwnerId &&
                !x.IsRevoked);

            if (hasFolderShare)
                return true;
        }
        return false;
    }

    public async Task<Result> ToggleFavoriteStatusAsync(Guid fileId, string userId, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files
            .FirstOrDefaultAsync(x => x.Id == fileId && x.OwnerId == userId, cancellationToken);

        if (file is null)
            return Result.Failure(FileErrors.FileNotFound);

        file.IsFavorite = !file.IsFavorite;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
