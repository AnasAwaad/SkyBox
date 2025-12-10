namespace SkyBox.API.Contracts.Subscription;

public record MySubscriptionInfoResponse(
    string Plan,
    string UsedStorage,
    string StorageLimit,
    string RemainingStorage
);