using SkyBox.API.Contracts.SharedLink;
using SkyBox.API.Persistence;
using System.Security.Cryptography;
using System.Linq.Dynamic.Core;


namespace SkyBox.API.Services;

public class SharedLinkService(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment) : ISharedLinkService
{
    private readonly string _filesPath = $"{webHostEnvironment.WebRootPath}/uploads";


    public async Task<Result<PaginatedList<SharedLinkResponse>>> GetMyLinksAsync(string userId, RequestFilters filters, CancellationToken cancellationToken)
    {
        var query = dbContext.SharedLinks
            .AsNoTracking()
            .Include(s => s.File)
            .Where(s => s.OwnerId == userId && s.IsActive);

        if (!string.IsNullOrEmpty(filters.SearchValue))
            query = query.Where(x => x.File.FileName.ToLower().Contains(filters.SearchValue.Trim().ToLower()));

        if (!string.IsNullOrEmpty(filters.SortColumn))
            query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");

        var source = query
            .ProjectToType<SharedLinkResponse>()
            .AsNoTracking();

        var result = await PaginatedList<SharedLinkResponse>.CreateAsync(source, filters.PageNumber, filters.PageSize, cancellationToken);

        return Result.Success(result);

    }


    public async Task<Result<SharedLinkResponse>> CreateSharedLinkAsync(Guid fileId, string ownerId, CreateSharedLinkRequest request, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files
            .FirstOrDefaultAsync(f => f.Id == fileId && f.OwnerId == ownerId, cancellationToken);

        if (file is null)
            return Result.Failure<SharedLinkResponse>(FileErrors.FileNotFound);

        var token = GenerateToken();

        var link = request.Adapt<SharedLink>();
        link.FileId = fileId;
        link.OwnerId = ownerId;
        link.Token = token;


        await dbContext.SharedLinks.AddAsync(link,cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = link.Adapt<SharedLinkResponse>();

        response.FileName = file.FileName;
        response.FileSize = file.Size;


        return Result.Success(response);

    }

    public async Task<Result> DeleteAsync(Guid sharedLinkId,string ownerId,CancellationToken cancellationToken = default)
    {
        var link = await dbContext.SharedLinks
            .FirstOrDefaultAsync(s => s.Id == sharedLinkId && s.OwnerId == ownerId, cancellationToken);

        if (link is null)
            return Result.Failure(SharedLinkErrors.SharedLinkNotFound);

        dbContext.SharedLinks.Remove(link);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
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

    public async Task<Result<StreamContentDto>> StreamByTokenAsync(string token,CancellationToken cancellationToken = default)
    {
        var link = await dbContext.SharedLinks
            .Include(s => s.File)
            .FirstOrDefaultAsync(s => s.Token == token && s.IsActive, cancellationToken);

        if (link is null || link.File.DeletedAt != null)
            return Result.Failure<StreamContentDto>(SharedLinkErrors.SharedLinkNotFound);

        if (IsExpired(link))
            return Result.Failure<StreamContentDto>(SharedLinkErrors.SharedLinkExpired);

        var file = link.File;
        var path = Path.Combine(_filesPath, file.StoredFileName);

        if (!File.Exists(path))
            return Result.Failure<StreamContentDto>(FileErrors.FileNotFound);

        var stream = File.OpenRead(path);

        link.Views += 1;
        await dbContext.SaveChangesAsync(cancellationToken);

        var dto = new StreamContentDto
        {
            Stream = stream,
            ContentType = file.ContentType,
            FileName = file.FileName
        };

        return Result.Success(dto);
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
        if (link.Permission != "download")
            return false;


        return link.MaxDownloads is null || link.Downloads < link.MaxDownloads;
    }

}
