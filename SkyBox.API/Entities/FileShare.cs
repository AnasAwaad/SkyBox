namespace SkyBox.API.Entities;

public class FileShare
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid FileId { get; set; }
    public UploadedFile File { get; set; } = default!;

    public string OwnerId { get; set; } = string.Empty;
    public ApplicationUser Owner { get; set; } = default!;

    public string SharedWithUserId { get; set; } = string.Empty;
    public ApplicationUser SharedWithUser { get; set; } = default!;

    public SharePermission Permission { get; set; } = SharePermission.View;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsRevoked { get; set; } = false;
}
