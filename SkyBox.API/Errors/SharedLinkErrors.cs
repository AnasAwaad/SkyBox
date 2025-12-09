namespace SkyBox.API.Errors;

public static class SharedLinkErrors
{
    public static readonly Error SharedLinkNotFound = new("SharedLink.NotFound", "No shared link was found with the given Id", StatusCodes.Status404NotFound);
    public static readonly Error SharedLinkExpired = new("SharedLink.Expired", "Shared link expired", StatusCodes.Status404NotFound);
    public static readonly Error DownloadLimitExceeded = new("SharedLink.DownloadLimitExceeded", "You have reached the maximum number of allowed downloads for this file", StatusCodes.Status404NotFound);

}
