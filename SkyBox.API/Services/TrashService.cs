
using Microsoft.EntityFrameworkCore;
using SkyBox.API.Errors;
using SkyBox.API.Persistence;
using SkyBox.API.Settings;
using System.Linq.Dynamic.Core;

namespace SkyBox.API.Services;

public class TrashService(ApplicationDbContext dbContext,
    IWebHostEnvironment webHostEnvironment,
    ILogger<TrashService> logger) : ITrashService
{
    private readonly string _filesPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads");

    public async Task<Result<PaginatedList<TrashedFileResponse>>> GetTrashFilesAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var threshold = DateTime.UtcNow.AddDays(-FileSettings.TrashRetentionDays);

        var query = dbContext.Files
            .Where(x => x.DeletedAt != null && x.DeletedAt >= threshold)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filters.SearchValue))
            query = query.Where(x => x.FileName.Contains(filters.SearchValue.ToLower()));

        if (!string.IsNullOrEmpty(filters.SortColumn))
            query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");

        var source = query
            .Select(x => new TrashedFileResponse
            {
                Id = x.Id,
                FileName = x.FileName,
                Size = x.Size,
                ContentType = x.ContentType,
                DeletedAt = x.DeletedAt!.Value,
                DaysRemaining = (int)Math.Ceiling(x.DeletedAt.Value.AddDays(FileSettings.TrashRetentionDays).Subtract(DateTime.UtcNow).TotalDays), // permanentDeleteDate - today
                PermanentDeleteDate = x.DeletedAt.Value.AddDays(FileSettings.TrashRetentionDays)
            })
            .AsNoTracking();

        var result = await PaginatedList<TrashedFileResponse>.CreateAsync(source, filters.PageNumber, filters.PageSize, cancellationToken);

        return Result.Success(result);
    }


    public async Task<Result> RestoreAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files
            .FirstOrDefaultAsync(x => x.Id == fileId && x.DeletedAt != null, cancellationToken);

        if (file is null)
            return Result.Failure(FileErrors.FileNotFound);

        var permanentDeleteDate = file.DeletedAt!.Value.AddDays(FileSettings.TrashRetentionDays);

        if (permanentDeleteDate < DateTime.UtcNow)
            return Result.Failure(FileErrors.FileExpired);

        file.DeletedAt = null;

        // file.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }


    public async Task<Result> PermanentlyDeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var file = await dbContext.Files
            .FirstOrDefaultAsync(x => x.Id == fileId && x.DeletedAt != null);

        if (file is null)
            return Result.Failure(FileErrors.FileNotFound);

        //if (!string.IsNullOrEmpty(userId))
        //    file = file.Where(x => x.OwnerId == userId);

        var filePath = Path.Combine(_filesPath, file.StoredFileName);
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                logger.LogInformation("Deleted file from storage: {FilePath}", filePath);
            }
            catch
            {
                logger.LogError("Failed to delete file from storage: {FilePath}", filePath);
            }
        }

        dbContext.Files.Remove(file);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();


    }

    public async Task<Result<int>> EmptyTrashAsync(CancellationToken cancellationToken = default)
    {
        var trashedFiles = await dbContext.Files
            .Where(x => x.DeletedAt != null)
            .ToListAsync(cancellationToken);

        if (trashedFiles.Count == 0)
            return Result.Success(0);

        foreach (var file in trashedFiles)
        {
            var filePath = Path.Combine(_filesPath, file.StoredFileName);
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    logger.LogError("Failed to delete file from storage: {FilePath}", filePath);
                }
            }

            dbContext.Files.Remove(file);
        }

        var affected = await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(affected);
    }

    public async Task<int> PermanentlyDeleteExpiredAsync()
    {
        var threshold = DateTime.UtcNow.AddDays(-FileSettings.TrashRetentionDays);

        var expired = await dbContext.Files
            .Where(f => f.DeletedAt != null && f.DeletedAt <= threshold)
            .ToListAsync();

        foreach (var file in expired)
        {
            var fullPath = Path.Combine(_filesPath, file.StoredFileName);

            if (File.Exists(fullPath))
            {
                try 
                { 
                    File.Delete(fullPath);
                }
                catch
                {
                    logger.LogError("Failed to delete file from storage: {FullPath}", fullPath);
                }
            }

            dbContext.Files.Remove(file);
        }

        return await dbContext.SaveChangesAsync();
    }
}
