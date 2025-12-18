namespace SkyBox.API.Contracts.SharedWithMe;

public class SharedWithMeQueryItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool IsFolder { get; set; }
    public SharePermission Permission { get; set; }
    public string OwnerName { get; set; } = default!;
    public DateTime SharedAt { get; set; }
    public string? ContentType { get; set; }
    public long? Size { get; set; }
}
