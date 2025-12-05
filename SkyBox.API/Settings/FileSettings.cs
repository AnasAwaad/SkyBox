namespace SkyBox.API.Settings;

public static class FileSettings
{
    public const int MaxFileSizeInMB = 10;
    public const int MaxFileSizeInBytes = MaxFileSizeInMB * 1024 * 1024; // 10 MB
    public static readonly string[] BlockedSignitures = ["4D-5A","2F-2A","D0-CF"];
}
