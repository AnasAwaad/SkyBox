namespace SkyBox.API.Contracts;

public class StreamContentDto
{
    public FileStream Stream { get; set; }
    public string ContentType { get; set; } = null!;
    public string FileName { get; set; } = null!;
}
