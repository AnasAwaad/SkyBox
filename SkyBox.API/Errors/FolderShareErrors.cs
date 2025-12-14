namespace SkyBox.API.Errors;

public static class FolderShareErrors
{
    public static readonly Error PermissionDenied = new("FolderShare.PermissionDenied", "You do not have permission to access this folder.", StatusCodes.Status403Forbidden);

}
