using System.Linq.Dynamic.Core;

namespace SkyBox.API.Services;

public class FileService(IStorageQuotaService storageQuotaService,
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

        // Save file
        var uploadedFile = await SaveFileAsync(file,userId,folderId, cancellationToken);

        await dbContext.AddAsync(uploadedFile, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(uploadedFile.Adapt<FileUploadResponse>());
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

        foreach (var file in files)
        {
            uploadedFiles.Add(await SaveFileAsync(file,userId,folderId, cancellationToken));
        }

        await dbContext.AddRangeAsync(uploadedFiles, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(uploadedFiles.Adapt<IEnumerable<FileUploadResponse>>());
    }


    public async Task<Result<FileContentDto>> DownloadAsync(Guid fileId,string userId, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files
            .FirstOrDefaultAsync(f => f.Id == fileId && f.OwnerId == userId, cancellationToken);

        if (file == null || file.DeletedAt != null)
            return Result.Failure<FileContentDto>(FileErrors.FileNotFound);

        var path = Path.Combine(_filesPath, file.StoredFileName);

        MemoryStream memoryStream = new();
        using FileStream fileStream = new(path, FileMode.Open);

        await fileStream.CopyToAsync(memoryStream, cancellationToken);

        memoryStream.Position = 0;

        var result = new FileContentDto
        {
            Content = memoryStream.ToArray(),
            ContentType = file.ContentType,
            FileName = file.FileName
        };

        return Result.Success(result);

    }

    public async Task<Result<StreamContentDto>> StreamAsync(Guid fileId,string userId, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files
            .FirstOrDefaultAsync(f => f.Id == fileId && f.OwnerId == userId, cancellationToken);

        if (file == null || file.DeletedAt != null)
            return Result.Failure<StreamContentDto>(FileErrors.FileNotFound);

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

    private async Task<UploadedFile> SaveFileAsync(IFormFile file,string userId, Guid? folderId, CancellationToken cancellationToken = default)
    {
        var randomFileName = Path.GetRandomFileName();

        var uploadedFile = new UploadedFile
        {
            FileName = file.FileName,
            StoredFileName = randomFileName,
            ContentType = file.ContentType,
            FileExtension = Path.GetExtension(file.FileName),
            Size = file.Length,
            UploadedAt = DateTime.UtcNow,
            FolderId = folderId,
            OwnerId = userId
        };

        var path = Path.Combine(_filesPath, randomFileName);
        using var stream = File.Create(path);
        await file.CopyToAsync(stream, cancellationToken);

        return uploadedFile;
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
}
