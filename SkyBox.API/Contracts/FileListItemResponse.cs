namespace SkyBox.API.Contracts;

public class FileListItemResponse
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? FolderId { get; set; }
    public bool IsFavorite { get; set; }
}
