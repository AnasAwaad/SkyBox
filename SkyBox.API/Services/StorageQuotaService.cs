using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using SkyBox.API.Helpers;

namespace SkyBox.API.Services;

public class StorageQuotaService(ApplicationDbContext dbContext,UserManager<ApplicationUser> userManager) : IStorageQuotaService
{
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
}
