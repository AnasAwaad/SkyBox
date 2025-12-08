
using SkyBox.API.Errors;
using SkyBox.API.Persistence;
using System.Linq.Dynamic.Core;

namespace SkyBox.API.Services;

public class FileService(IWebHostEnvironment webHostEnvironment,
    ApplicationDbContext dbContext) : IFileService
{
    private readonly string _filesPath = $"{webHostEnvironment.WebRootPath}/uploads";


    public async Task<Result<PaginatedList<FileListItemResponse>>> GetFilesAsync(RequestFilters filters,string userId, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Files
            .Where(x => x.DeletedAt == null && x.OwnerId == userId)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filters.SearchValue))
            query = query.Where(x => x.FileName.Contains(filters.SearchValue.ToLower()));

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

        if(folderId is not null)
        {
            var folder = await dbContext.Folders
                .FirstOrDefaultAsync(f => f.Id == folderId && f.OwnerId == userId, cancellationToken);


            if (folder is null)
                return Result.Failure<FileUploadResponse>(FolderErrors.FolderNotFound);
        }

        var uploadedFile = await SaveFileAsync(file,userId,folderId, cancellationToken);

        await dbContext.AddAsync(uploadedFile, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(uploadedFile.Adapt<FileUploadResponse>());
    }

    public async Task<Result<IEnumerable<FileUploadResponse>>> UploadManyAsync(IFormFileCollection files,string userId, Guid? folderId = null, CancellationToken cancellationToken = default)
    {
        if (folderId is not null)
        {
            var folder = await dbContext.Folders
                .FirstOrDefaultAsync(f => f.Id == folderId && f.OwnerId == userId, cancellationToken);

            if (folder is null)
                return Result.Failure<IEnumerable<FileUploadResponse>>(FolderErrors.FolderNotFound);

        }

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
}
