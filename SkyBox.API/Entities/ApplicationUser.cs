using Microsoft.AspNetCore.Identity;
using SkyBox.API.Enums;

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
    public SubscriptionPlan SubscriptionPlan { get; set; } = SubscriptionPlan.Free;
    public ICollection<UploadedFile> Files { get; set; } = [];
    public ICollection<Folder> Folders { get; set; } = [];
    public ICollection<SharedLink> SharedLinks { get; set; } = [];
}
