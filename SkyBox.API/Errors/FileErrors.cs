namespace SkyBox.API.Errors;

public static class FileErrors
{
    public static readonly Error FileNotFound = new("File.NotFound", "The requested file does not exist or you don't have access to it.", StatusCodes.Status404NotFound);
    public static readonly Error FileExpired = new("File.Expired", "The file can no longer be restored because its retention period has expired.", StatusCodes.Status400BadRequest);
    public static readonly Error StorageQuotaExceeded = new("File.StorageQuotaExceeded", "Cannot upload the file because it exceeds your current storage plan. Please delete files or upgrade your subscription.", StatusCodes.Status400BadRequest);
    public static readonly Error EmptyFile = new("File.EmptyFile", "The uploaded file is empty. Please provide a valid file.", StatusCodes.Status400BadRequest);
    public static readonly Error NoFilesProvided = new("File.NoFilesProvided", "No files were provided for upload. Please attach at least one file.", StatusCodes.Status400BadRequest);
    public static readonly Error EmptyFilesOnly = new("File.EmptyFilesOnly", "All provided files are empty. Please upload at least one non-empty file.", StatusCodes.Status400BadRequest);
    public static readonly Error VersioningNotAllowed = new("File.VersioningNotAllowed", "Versioning is available for Business plan only.", StatusCodes.Status403Forbidden);
    public static readonly Error Forbidden = new("File.Forbidden", "You are not the owner of this file.", StatusCodes.Status403Forbidden);
    public static readonly Error StorageMissing = new("File.StorageMissing", "Version file missing from storage.", StatusCodes.Status500InternalServerError);
    public static readonly Error VersionAlreadyDeleted = new("File.VersionAlreadyDeleted", "File Version is already deleted.", StatusCodes.Status400BadRequest);
}

