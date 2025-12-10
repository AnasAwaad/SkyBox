namespace SkyBox.API.Helpers;

public static class SubscriptionPlanLimits
{
    private const long OneGB = 1L * 1024 * 1024 * 1024;

    /// <summary>
    /// Returns max allowed storage in bytes or null if it is unlimited.
    /// </summary>
    public static long? GetStorageLimitBytes(SubscriptionPlan plan)
    {
        return plan switch
        {
            SubscriptionPlan.Free => 5 * OneGB,
            SubscriptionPlan.Premium => 50 * OneGB,
            SubscriptionPlan.Business => null,
            _ => throw new ArgumentOutOfRangeException(nameof(plan), "Unknown subscription plan")
        };
    }
}
