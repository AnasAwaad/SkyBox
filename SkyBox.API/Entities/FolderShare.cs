namespace SkyBox.API.Entities;

public class FolderShare
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid FolderId { get; set; }
    public Folder Folder { get; set; } = default!;

    public string OwnerId { get; set; } = default!;
    public string SharedWithUserId { get; set; } = default!;

    public SharePermission Permission { get; set; } = SharePermission.View;

    public bool IsRevoked { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

