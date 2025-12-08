namespace SkyBox.API.Errors;

public static class FolderErrors
{
    public static readonly Error FolderNotFound = new("Folder.NotFound", "No folder was found with the given Id", StatusCodes.Status404NotFound);
    public static readonly Error ParentFolderNotFound = new("ParentFolder.NotFound", "No folder was found with the given Id", StatusCodes.Status404NotFound);
    public static readonly Error FolderExists = new("Folder.Exists", "Folder with the same name already exists.", StatusCodes.Status404NotFound);
}
