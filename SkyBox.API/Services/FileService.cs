
using SkyBox.API.Persistence;

namespace SkyBox.API.Services;

public class FileService(IWebHostEnvironment webHostEnvironment,
    ApplicationDbContext dbContext) : IFileService
{
    private readonly string _filesPath = $"{webHostEnvironment.WebRootPath}/uploads";


    public async Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        
        var uploadedFile =await SaveFileAsync(file, cancellationToken);

        await dbContext.AddAsync(uploadedFile,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return uploadedFile.Id;
    }

    public async Task<IEnumerable<Guid>> UploadManyAsync(IFormFileCollection files, CancellationToken cancellationToken = default)
    {
        List<UploadedFile> uploadedFiles = [];

        foreach (var file in files)
        {
            uploadedFiles.Add(await SaveFileAsync(file, cancellationToken));
        }

        await dbContext.AddRangeAsync(uploadedFiles,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return uploadedFiles.Select(x => x.Id);
    }


    public async Task<(byte[] fileContent, string contentType, string fileName)> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files.FindAsync(fileId);

        if (file == null)
            return ([], string.Empty, string.Empty);

        var path = Path.Combine(_filesPath,file.StoredFileName);

        MemoryStream memoryStream = new();
        using FileStream fileStream = new (path,FileMode.Open);

        await fileStream.CopyToAsync(memoryStream,cancellationToken);

        memoryStream.Position = 0;

        return (memoryStream.ToArray(),file.ContentType,file.FileName);

    }

    public async Task<(FileStream? stream, string contentType, string fileName)> StreamAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files.FindAsync(fileId);

        if (file == null)
            return (null, string.Empty, string.Empty);

        var path = Path.Combine(_filesPath, file.StoredFileName);

        var fileStream = File.OpenRead(path);

        return (fileStream, file.ContentType, file.FileName);
    }

    private async Task<UploadedFile> SaveFileAsync(IFormFile file,CancellationToken cancellationToken = default)
    {
        var randomFileName = Path.GetRandomFileName();

        var uploadedFile = new UploadedFile
        {
            FileName = file.FileName,
            StoredFileName = randomFileName,
            ContentType = file.ContentType,
            FileExtension = Path.GetExtension(file.FileName),
        };

        var path = Path.Combine(_filesPath, randomFileName);
        using var stream = File.Create(path);
        await file.CopyToAsync(stream, cancellationToken);

        return uploadedFile;
    }

}
