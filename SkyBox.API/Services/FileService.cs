
using SkyBox.API.Errors;
using SkyBox.API.Persistence;
using System.Linq.Dynamic.Core;

namespace SkyBox.API.Services;

public class FileService(IWebHostEnvironment webHostEnvironment,
    ApplicationDbContext dbContext) : IFileService
{
    private readonly string _filesPath = $"{webHostEnvironment.WebRootPath}/uploads";


    public async Task<Result<PaginatedList<FileListItemResponse>>> GetFilesAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Files.AsQueryable();

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

    public async Task<FileUploadResponse> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
    {

        var uploadedFile = await SaveFileAsync(file, cancellationToken);

        await dbContext.AddAsync(uploadedFile, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return uploadedFile.Adapt<FileUploadResponse>();
    }

    public async Task<IEnumerable<FileUploadResponse>> UploadManyAsync(IFormFileCollection files, CancellationToken cancellationToken = default)
    {
        List<UploadedFile> uploadedFiles = [];

        foreach (var file in files)
        {
            uploadedFiles.Add(await SaveFileAsync(file, cancellationToken));
        }

        await dbContext.AddRangeAsync(uploadedFiles, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return uploadedFiles.Adapt<IEnumerable<FileUploadResponse>>();
    }


    public async Task<Result<FileContentDto>> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files.FindAsync(fileId);

        if (file == null)
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

    public async Task<Result<StreamContentDto>> StreamAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files.FindAsync(fileId);

        if (file == null)
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

    private async Task<UploadedFile> SaveFileAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var randomFileName = Path.GetRandomFileName();

        var uploadedFile = new UploadedFile
        {
            FileName = file.FileName,
            StoredFileName = randomFileName,
            ContentType = file.ContentType,
            FileExtension = Path.GetExtension(file.FileName),
            Size = file.Length,
            UploadedAt = DateTime.UtcNow
        };

        var path = Path.Combine(_filesPath, randomFileName);
        using var stream = File.Create(path);
        await file.CopyToAsync(stream, cancellationToken);

        return uploadedFile;
    }

}
