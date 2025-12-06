namespace SkyBox.API.Contracts;

public class TrashedFileResponse
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime DeletedAt { get; set; }
    public int DaysRemaining { get; set; }
    public DateTime PermanentDeleteDate { get; set; }
}
