using SkyBox.API.Contracts.Subscription;

namespace SkyBox.API.Services;

public interface ISubscriptionService
{
    /// <summary>
    /// Change the subscription plan for a specific user.
    /// </summary>
    /// <remarks>
    /// Used by administrators to upgrade or downgrade user plans.
    /// Downgrade is not allowed if used storage exceeds the new plan limit.
    /// </remarks>
    Task<Result> ChangeUserPlan(string userId, SubscriptionPlan plan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the current user's subscription plan and storage usage.
    /// </summary>
    Task<MySubscriptionInfoResponse> GetMySubscriptionInfo(string userId, CancellationToken cancellationToken = default);
}
