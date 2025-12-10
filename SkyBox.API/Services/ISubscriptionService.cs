using SkyBox.API.Contracts.Subscription;

namespace SkyBox.API.Services;

public interface ISubscriptionService
{
    Task<Result> ChangeUserPlan(string userId, SubscriptionPlan plan, CancellationToken cancellationToken = default);
    Task<MySubscriptionInfoResponse> GetMySubscriptionInfo(string userId, CancellationToken cancellationToken = default);
}
