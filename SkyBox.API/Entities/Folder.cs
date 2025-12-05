using System.Security;

namespace SkyBox.API.Entities;

public class Folder
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string? OwnerId { get; set; }
    public ApplicationUser? Owner { get; set; }

    public string? ParentId { get; set; }
    public Folder? Parent { get; set; }
    public ICollection<Folder> Children { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsFavorite { get; set; } = false;
    public ICollection<UploadedFile> Files { get; set; } = [];
    //public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    public string? Path { get; set; }
}
