namespace SkyBox.API.Errors;

public static class SharedLinkErrors
{
    public static readonly Error SharedLinkNotFound = new("SharedLink.NotFound", "No shared link was found with the given Id", StatusCodes.Status404NotFound);
    public static readonly Error SharedLinkExpired = new("SharedLink.Expired", "Shared link expired", StatusCodes.Status404NotFound);
    public static readonly Error DownloadLimitExceeded = new("SharedLink.DownloadLimitExceeded", "You have reached the maximum number of allowed downloads for this file or you don't have permission to download this file ", StatusCodes.Status404NotFound);
    public static readonly Error DownloadNotAllowed = new("SharedLink.DownloadNotAllowed", "Downloading this file is not allowed.", StatusCodes.Status403Forbidden);
    public static readonly Error InvalidPassword = new("SharedLink.InvalidPassword", "The password you entered is incorrect.", StatusCodes.Status400BadRequest);
    public static readonly Error PermissionDenied = new("SharedLink.PermissionDenied", "You do not have permission to access this file.", StatusCodes.Status403Forbidden);
    public static readonly Error PasswordRequired = new("SharedLink.PasswordRequired", "This shared link is protected by a password.", StatusCodes.Status400BadRequest);

}
