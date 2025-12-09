using SkyBox.API.Contracts.SharedLink;
using SkyBox.API.Persistence;
using System.Security.Cryptography;

namespace SkyBox.API.Services;

public class SharedLinkService(ApplicationDbContext dbContext,IWebHostEnvironment webHostEnvironment) : ISharedLinkService
{
    private readonly string _filesPath = $"{webHostEnvironment.WebRootPath}/uploads";

    public async Task<Result<SharedLinkResponse>> CreateSharedLinkAsync(Guid fileId, string ownerId, CreateSharedLinkRequest request, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files
            .FirstOrDefaultAsync(f => f.Id == fileId && f.OwnerId == ownerId, cancellationToken);

        if (file is null)
            return Result.Failure<SharedLinkResponse>(FileErrors.FileNotFound);

        var token = GenerateToken();

        var sharedLink = request.Adapt<SharedLink>();
        sharedLink.FileId = fileId;
        sharedLink.OwnerId = ownerId;
        sharedLink.Token = token;


        await dbContext.SharedLinks.AddAsync(sharedLink,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = sharedLink.Adapt<SharedLinkResponse>();

        response.Url = $"/share/{sharedLink.Token}";
        response.FileName = file.FileName;
        response.FileSize = file.Size;


        return Result.Success(response);

    }
    public async Task<Result<FileContentDto>> DownloadByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var link =await dbContext.SharedLinks
            .Include(sl=>sl.File)
            .FirstOrDefaultAsync(sl => sl.Token == token && sl.IsActive, cancellationToken);

        if (link is null)
            return Result.Failure<FileContentDto>(SharedLinkErrors.SharedLinkNotFound);

        if(IsExpired(link))
            return Result.Failure<FileContentDto>(SharedLinkErrors.SharedLinkExpired);
        
        if(!CanDownload(link))
            return Result.Failure<FileContentDto>(SharedLinkErrors.DownloadLimitExceeded);

        var file = link.File;
        var path = Path.Combine(_filesPath, file.StoredFileName);

        if(!File.Exists(path))
            return Result.Failure<FileContentDto>(FileErrors.FileNotFound);


        MemoryStream memoryStream = new();
        using FileStream fileStream = new(path, FileMode.Open);

        await fileStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        link.Downloads += 1;
        await dbContext.SaveChangesAsync(cancellationToken);

        var result = new FileContentDto
        {
            Content = memoryStream.ToArray(),
            ContentType = file.ContentType,
            FileName = file.FileName
        };

        return Result.Success(result);
    }

    public async Task<Result<SharedLinkPublicInfoResponse>> GetInfoByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var link = await dbContext.SharedLinks
            .Include(s => s.File)
            .FirstOrDefaultAsync(s => s.Token == token && s.IsActive, cancellationToken);

        if (link is null || link.File.DeletedAt != null)
            return Result.Failure<SharedLinkPublicInfoResponse>(SharedLinkErrors.SharedLinkNotFound);

        if (IsExpired(link))
            return Result.Failure<SharedLinkPublicInfoResponse>(SharedLinkErrors.SharedLinkExpired);

        var file = link.File;

        var response = new SharedLinkPublicInfoResponse(file.FileName, file.Size,file.ContentType, link.ExpiresAt, link.Permission, IsExpired(link));

        link.Views += 1;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(response);
    }

    private static string GenerateToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
    }

    private static bool IsExpired(SharedLink link)
    {
        return link.ExpiresAt < DateTime.UtcNow;
    }

    private static bool CanDownload(SharedLink link)
    {
        return link.MaxDownloads is null || link.Downloads < link.MaxDownloads;
    }

}
