namespace SkyBox.API.Entities;

public class SharedLink
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public UploadedFile File { get; set; } = default!;

    public string OwnerId { get; set; } = string.Empty;
    public ApplicationUser Owner { get; set; } = default!;

    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } 

    public int? MaxDownloads { get; set; }
    public int Downloads { get; set; } = 0;
    public int Views { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public string Permission { get; set; } = "view";

    public string? PasswordHash { get; set; }
}
