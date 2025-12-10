
using Microsoft.AspNetCore.Identity;

namespace SkyBox.API.Services;

public class SubscriptionService(UserManager<ApplicationUser> userManager,IStorageQuotaService storageQuotaService) : ISubscriptionService
{
    public async Task<Result> ChangeUserPlan(string userId, SubscriptionPlan plan, CancellationToken cancellationToken = default)
    {
        var user =await userManager.FindByIdAsync(userId);

        if(user is null)
            return Result.Failure(UserErrors.UserNotFound);

        var currentPlan = user.SubscriptionPlan;

        if(currentPlan == plan)
            return Result.Failure(SubscriptionErrors.AlreadyOnThisPlan);

        var usedStorage = await storageQuotaService.GetUsedStorageAsync(userId, cancellationToken);

        var newPlanLimit = SubscriptionPlanLimits.GetStorageLimitBytes(plan);

        if(newPlanLimit is not null && newPlanLimit < usedStorage)
            return Result.Failure(SubscriptionErrors.CannotDowngradeBelowUsedStorage);

        user.SubscriptionPlan = plan;
        var result = await userManager.UpdateAsync(user);

        if(!result.Succeeded)
            return Result.Failure(SubscriptionErrors.FailedToUpdatePlan);

        return Result.Success();
    }
}
