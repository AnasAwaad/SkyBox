namespace SkyBox.API.Helpers;

public static class StorageSizeFormatter
{
    public static string ToReadableSize(this long bytes)
    {
        if(bytes < 1024)
            return $"{bytes} B";

        double kiloBytes = bytes / 1024.0;
        if(kiloBytes < 1024)
            return $"{kiloBytes:F2} KB";

        double megaBytes = kiloBytes / 1024.0;
        if(megaBytes < 1024)
            return $"{megaBytes:F2} MB";

        double gigaBytes = megaBytes / 1024.0;
        return $"{gigaBytes:F2} GB";
    }

    public static string ToReadableSize(this long? bytes)
    {
        if (bytes is null)
            return "Unlimited";

        return ToReadableSize(bytes.Value);
    }
}
