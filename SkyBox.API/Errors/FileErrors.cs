namespace SkyBox.API.Errors;

public static class FileErrors
{
    public static readonly Error FileNotFound = new("File.NotFound", "No file was found with the given Id", StatusCodes.Status404NotFound);
    public static readonly Error FileExpired = new("File.Expired", "File can no longer be restored (expired from trash).", StatusCodes.Status400BadRequest);
}

