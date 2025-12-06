namespace SkyBox.API.Errors;

public static class FileErrors
{
    public static readonly Error FileNotFound = new("File.NotFound", "No file was found with the given Id", StatusCodes.Status404NotFound);
}

