namespace SkyBox.API.Contracts.SharedLink;

public class SharedLinkResponse
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int Downloads { get; set; }
    public int Views { get; set; }
    public string Url { get; set; } = string.Empty;
}