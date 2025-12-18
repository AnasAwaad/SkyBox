namespace SkyBox.API.Contracts.Files;

public class FileContentDto
{
    public Stream Content { get; set; } = default!;
    public string ContentType { get; set; } = null!;
    public string FileName { get; set; } = null!;
}
