namespace SkyBox.API.Contracts.Files;

public class FileContentDto
{
    public byte[] Content { get; set; } = [];
    public string ContentType { get; set; } = null!;
    public string FileName { get; set; } = null!;
}
