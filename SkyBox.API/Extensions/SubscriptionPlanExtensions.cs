namespace SkyBox.API.Extensions;

public static class SubscriptionPlanExtensions
{
    public static string ToDisplayName(this SubscriptionPlan plan)
    {
        return plan switch
        {
            SubscriptionPlan.Free => "Free Plan",
            SubscriptionPlan.Premium => "Premium Plan",
            SubscriptionPlan.Business => "Business Plan",
            _ => "Unknown"
        };
    }
}
