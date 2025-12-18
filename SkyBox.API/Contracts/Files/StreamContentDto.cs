namespace SkyBox.API.Contracts.Files;

public class StreamContentDto
{
    public Stream Stream { get; set; }
    public string ContentType { get; set; } = null!;
    public string FileName { get; set; } = null!;
}
