namespace SkyBox.API.Errors;

public class FileShareErrors
{
    public static readonly Error ShareNotFound =new("FileShare.NotFound","Shared file not found.",StatusCodes.Status404NotFound);
    public static readonly Error PermissionDenied =new("FileShare.PermissionDenied","You do not have permission to access this file.",StatusCodes.Status403Forbidden);
    public static readonly Error AlreadyShared =new("FileShare.AlreadyShared","This file is already shared with this user.",StatusCodes.Status409Conflict);
    public static readonly Error InvalidUser =new("FileShare.InvalidUser","The selected user does not exist.",StatusCodes.Status400BadRequest);
    public static readonly Error OwnerOnly =new("FileShare.OwnerOnly","Only the file owner can perform this action.",StatusCodes.Status403Forbidden);
    public static readonly Error AccessRevoked =new("FileShare.AccessRevoked","Your access to this file has been revoked.",StatusCodes.Status403Forbidden);
}
