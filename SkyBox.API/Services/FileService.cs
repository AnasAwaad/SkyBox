using Microsoft.EntityFrameworkCore;
using SkyBox.API.Contracts.FileVersions;
using System.Linq.Dynamic.Core;

namespace SkyBox.API.Services;

public class FileService(IStorageQuotaService storageQuotaService,
    IFileVersionService fileVersionService,
    IWebHostEnvironment webHostEnvironment,
    ApplicationDbContext dbContext) : IFileService
{
    private readonly string _filesPath = $"{webHostEnvironment.WebRootPath}/uploads";


    public async Task<Result<PaginatedList<FileListItemResponse>>> GetFilesAsync(RequestFilters filters,string userId, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Files
            .Where(x => x.DeletedAt == null && x.OwnerId == userId)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filters.SearchValue))
            query = query.Where(x => x.FileName.Contains(filters.SearchValue, StringComparison.CurrentCultureIgnoreCase));

        if (!string.IsNullOrEmpty(filters.SortColumn))
            query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");

        var source = query
            .ProjectToType<FileListItemResponse>()
            .AsNoTracking();

        var result = await PaginatedList<FileListItemResponse>.CreateAsync(source, filters.PageNumber, filters.PageSize, cancellationToken);

        return Result.Success(result);

    }

    public async Task<Result<FileUploadResponse>> UploadAsync(IFormFile file,string userId, Guid? folderId = null, CancellationToken cancellationToken = default)
    {
        // Validate file
        if (file is null || file.Length <= 0)
            return Result.Failure<FileUploadResponse>(FileErrors.EmptyFile);

        
        // Validate folder ownership
        var folderResult = await EnsureFolderBelongsToUserAsync(folderId, userId, cancellationToken);
        if (folderResult.IsFailure)
            return Result.Failure<FileUploadResponse>(folderResult.Error);


        // Check storage quota
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
        // Validate files
        if (files is null || files.Count == 0)
            return Result.Failure<IEnumerable<FileUploadResponse>>(FileErrors.NoFilesProvided);

        // Filter out empty files
        var nonEmptyFiles = files.Where(f => f is not null && f.Length > 0).ToList();
        if (!nonEmptyFiles.Any())
            return Result.Failure<IEnumerable<FileUploadResponse>>(FileErrors.EmptyFilesOnly);


        // Validate folder ownership 
        var folderResult = await EnsureFolderBelongsToUserAsync(folderId, userId, cancellationToken);
        if (folderResult.IsFailure)
            return Result.Failure<IEnumerable<FileUploadResponse>>(folderResult.Error);


        // Check storage quota
        var totalSize = files.Sum(x=>x.Length);
        var canUpload = await storageQuotaService.CanUploadFileAsync(userId, totalSize, cancellationToken);

        if (!canUpload)
            return Result.Failure<IEnumerable<FileUploadResponse>>(FileErrors.StorageQuotaExceeded);

        // Save all files
        List<UploadedFile> uploadedFiles = [];
        var responses = new List<FileUploadResponse>();

        foreach (var file in files)
        {
            var saveResult = await SaveOrCreateVersionAsync(file, userId, folderId, cancellationToken);


            if (saveResult.IsFailure)
                return Result.Failure<IEnumerable<FileUploadResponse>>(saveResult.Error);

            // version created & persisted by FileVersionService
            if (saveResult.Value.AlreadyPersisted)
            {
                responses.Add(saveResult.Value.File.Adapt<FileUploadResponse>());
                continue;
            }

            // new file
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


        var content = await storageQuotaService.DownloadFileAsync(file.StoredFileName, cancellationToken);

        if(content == null)
            return Result.Failure<FileContentDto>(FileErrors.StorageMissing);


        var result = new FileContentDto
        {
            Content = content,
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

        var path = Path.Combine(_filesPath, file.StoredFileName);

        var fileStream = File.OpenRead(path);

        var result = new StreamContentDto
        {
            Stream = fileStream,
            ContentType = file.ContentType,
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
        // find existing file in same folder with same name
        var existingFile = await dbContext.Files
            .FirstOrDefaultAsync(f =>
                f.OwnerId == userId &&
                f.FolderId == folderId &&
                f.FileName == file.FileName &&
                f.DeletedAt == null,
                cancellationToken);

        if (existingFile is not null)
        {
            // create version via FileVersionService
            var versionResult = await fileVersionService.SaveNewVersionAsync(existingFile, file, userId, cancellationToken);
            if (versionResult.IsFailure)
                return Result.Failure<SaveOrVersionResult>(versionResult.Error);

            return Result.Success(new SaveOrVersionResult(versionResult.Value, true));
        }

        // store to storage and return UploadedFile
        var storedFileName = await storageQuotaService.UploadFileAsync(file, cancellationToken);

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

    private async Task<bool> CanAccessFileAsync(UploadedFile file, string userId)
    {
        // Owner of the file
        if (file.OwnerId == userId)
            return true;

        // Direct file share
        var hasFileShare = await dbContext.FileShares
            .AnyAsync(x => x.FileId == file.Id && x.SharedWithUserId == userId && x.OwnerId == file.OwnerId && !x.IsRevoked);

        if(hasFileShare)
            return true;

        // Folder share
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

}
