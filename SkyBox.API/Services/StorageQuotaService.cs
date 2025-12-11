using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using SkyBox.API.Helpers;

namespace SkyBox.API.Services;

public class StorageQuotaService(ApplicationDbContext dbContext,IWebHostEnvironment webHostEnvironment,UserManager<ApplicationUser> userManager) : IStorageQuotaService
{
    private readonly string _filesPath = $"{webHostEnvironment.WebRootPath}/uploads";

    public async Task<bool> CanUploadFileAsync(string userId, long fileSizeBytes, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);

        if(user is null)
            return false;

        var usedStorage = await GetUsedStorageAsync(userId, cancellationToken);

        var limitBytes = SubscriptionPlanLimits.GetStorageLimitBytes(user.SubscriptionPlan);

        if(limitBytes is null)
            return true;

        return (usedStorage + fileSizeBytes) <= limitBytes;
    }

    public Task<long> GetUsedStorageAsync(string userId, CancellationToken cancellationToken = default)
    {
        return dbContext.Files
            .Where(x=>x.OwnerId == userId)
            .SumAsync(x=>x.Size, cancellationToken);
    }

    public async Task<string> UploadFileAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var randomFileName = Path.GetRandomFileName();

        var path = Path.Combine(_filesPath, randomFileName);
        using var stream = File.Create(path);
        await file.CopyToAsync(stream, cancellationToken);

        return randomFileName;
    }
}
