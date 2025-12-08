using Microsoft.AspNetCore.Identity;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace SkyBox.API.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime? SuspendedAt { get; set; }
    public string? SuspendedReason { get; set; }
    public string? SuspendedBy { get; set; }
    public long StorageQuotaBytes { get; set; } = 10737418240; // 10GB
    public long UsedStorageBytes { get; set; } = 0;

    public ICollection<UploadedFile> Files { get; set; } = [];
    public ICollection<Folder> Folders { get; set; } = [];
}
