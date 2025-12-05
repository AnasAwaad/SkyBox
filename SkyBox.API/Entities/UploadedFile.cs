namespace SkyBox.API.Entities;

public class UploadedFile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;

    //public string OwnerId { get; set; } = string.Empty;
    //public ApplicationUser Owner { get; set; } = default!;

    public string? FolderId { get; set; }
    public Folder? Folder { get; set; }

    //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsFavorite { get; set; } = false;
    //public DateTime? DeletedAt { get; set; }
}
