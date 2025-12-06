using System.Security;

namespace SkyBox.API.Entities;

public class Folder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public string? OwnerId { get; set; }
    public ApplicationUser? Owner { get; set; }

    public Guid? ParentId { get; set; }
    public Folder? Parent { get; set; }
    public ICollection<Folder> Folders { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsFavorite { get; set; } = false;
    public ICollection<UploadedFile> Files { get; set; } = [];
    public string? Path { get; set; }
}
