using Microsoft.AspNetCore.Mvc;

namespace SkyBox.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FilesController(IFileService fileService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetFiles([FromQuery]RequestFilters filters,CancellationToken cancellationToken)
    {
        var result = await fileService.GetFilesAsync(filters, cancellationToken);
        return Ok(result.Value);
    }

    [HttpPost("upload")]
    public async Task<ActionResult<FileUploadResponse>> UploadFile([FromForm] UploadFileRequest request, CancellationToken cancellationToken)
    {
        var file = await fileService.UploadAsync(request.File, cancellationToken);
        return CreatedAtAction(nameof(DownloadFile), new { id = file.Id }, file);
    }

    [HttpPost("upload-many")]
    public async Task<ActionResult<IEnumerable<FileUploadResponse>>> UploadManyFiles([FromForm] UploadManyFilesRequest request, CancellationToken cancellationToken)
    {
        var files = await fileService.UploadManyAsync(request.Files, cancellationToken);
        return Ok(files);
    }

    [HttpGet("download/{id}")]
    public async Task<IActionResult> DownloadFile([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await fileService.DownloadAsync(id, cancellationToken);

        return result.IsSuccess ?
            File(result.Value.Content, result.Value.ContentType, result.Value.FileName) :
            result.ToProblem();
    }

    [HttpGet("stream/{id}")]
    public async Task<IActionResult> Stream([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await fileService.StreamAsync(id, cancellationToken);

        return result.IsSuccess ?
            File(result.Value.Stream, result.Value.ContentType, result.Value.FileName) :
            result.ToProblem();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await fileService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ?
            NoContent() :
            result.ToProblem();
    }
}
