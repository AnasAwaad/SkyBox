namespace SkyBox.API.Contracts.Subscription;

public record ChangeUserPlanRequest(
    string UserId,
    SubscriptionPlan Plan
);
