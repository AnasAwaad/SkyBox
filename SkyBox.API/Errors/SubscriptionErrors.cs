namespace SkyBox.API.Errors;

public static class SubscriptionErrors
{
    public static readonly Error AlreadyOnThisPlan = new("Subscription.AlreadyOnThisPlan","The user is already on the requested plan.",StatusCodes.Status400BadRequest);

    public static readonly Error CannotDowngradeBelowUsedStorage = new("Subscription.CannotDowngradeBelowUsedStorage","The user is using more storage than the new plan would allow.",StatusCodes.Status400BadRequest);

    public static readonly Error FailedToUpdatePlan = new("Subscription.FailedToUpdatePlan","Failed to update the subscription plan for the user.",StatusCodes.Status500InternalServerError);
}
