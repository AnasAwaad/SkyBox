namespace SkyBox.API.Services;

public interface ISubscriptionService
{
    Task<Result> ChangeUserPlan(string userId, SubscriptionPlan plan, CancellationToken cancellationToken = default);
}
