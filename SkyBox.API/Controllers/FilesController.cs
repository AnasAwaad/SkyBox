using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts;

namespace SkyBox.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FilesController(IFileService fileService) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] UploadFileRequest request, CancellationToken cancellationToken)
    {
        var fileId = await fileService.UploadAsync(request.File, cancellationToken);
        return CreatedAtAction(nameof(DownloadFile), new { id = fileId }, null);
    }

    [HttpPost("upload-many")]
    public async Task<ActionResult<IEnumerable<Guid>>> UploadManyFiles([FromForm] UploadManyFilesRequest request, CancellationToken cancellationToken)
    {
        var fileIds = await fileService.UploadManyAsync(request.Files, cancellationToken);
        return Ok(fileIds);
    }

    [HttpGet("download/{id}")]
    public async Task<IActionResult> DownloadFile([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var (fileContent,contentType,fileName) = await fileService.DownloadAsync(id, cancellationToken);

        if (fileContent is [])
            return NotFound();

        return File(fileContent, contentType, fileName);
    }

    [HttpGet("stream/{id}")]
    public async Task<IActionResult> Stream([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var (stream, contentType, fileName) = await fileService.StreamAsync(id, cancellationToken);

        if (stream is null)
            return NotFound();

        return File(stream, contentType, fileName,enableRangeProcessing:true);
    }
}
