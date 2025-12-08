namespace SkyBox.API.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "Invalid email or password", StatusCodes.Status401Unauthorized);
    public static readonly Error LockedUser = new("User.LockedUser", "Locked User, Please contact your administrator", StatusCodes.Status401Unauthorized);
    public static readonly Error DisabledUser = new("User.DisabledUser", "Disabled User, Please contact your administrator", StatusCodes.Status401Unauthorized);
    public static readonly Error EmailNotConfirmed = new("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status400BadRequest);
    public static readonly Error UserNotFound = new("User.NotFound", "No user was found with the given Id", StatusCodes.Status404NotFound);
    public static readonly Error InvalidJwtToken = new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidRefreshToken = new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status400BadRequest);
    public static readonly Error DuplicatedEmail = new("User.DuplicatedEmail", "Another user with the same email is already exists", StatusCodes.Status409Conflict);
    public static readonly Error InvalidCode = new("User.InvalidCode", "Invalid code", StatusCodes.Status400BadRequest);
    public static readonly Error EmailAlreadyConfirmed = new("User.EmailAlreadyConfirmed", "Email already confirmed", StatusCodes.Status400BadRequest);
    public static readonly Error InvalidRoles = new("Role.InvalidRoles", "Invalid roles", StatusCodes.Status400BadRequest);

}

