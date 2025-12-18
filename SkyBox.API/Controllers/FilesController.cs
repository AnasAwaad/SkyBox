using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Abstractions.Consts;
using SkyBox.API.Contracts.Files;

namespace SkyBox.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = $"{DefaultRoles.User},{DefaultRoles.Admin}")]

public class FilesController(IFileService fileService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetFiles([FromQuery]RequestFilters filters,CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await fileService.GetFilesAsync(filters,userId, cancellationToken);
        return Ok(result.Value);
    }

    [HttpPost("upload")]
    public async Task<ActionResult<FileUploadResponse>> UploadFile([FromForm] UploadFileRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await fileService.UploadAsync(request.File, userId, request.FolderId, cancellationToken);
        return result.IsSuccess ? CreatedAtAction(nameof(DownloadFile), new { id = result.Value.Id }, result.Value) : result.ToProblem();
    }

    [HttpPost("upload-many")]
    public async Task<ActionResult<IEnumerable<FileUploadResponse>>> UploadManyFiles([FromForm] UploadManyFilesRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await fileService.UploadManyAsync(request.Files,userId,request.FolderId, cancellationToken);
        return result.IsSuccess ? CreatedAtAction(nameof(GetFiles), null, result.Value) : result.ToProblem();
    }

    [HttpGet("download/{id}")]
    public async Task<IActionResult> DownloadFile([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await fileService.DownloadAsync(id,userId, cancellationToken);

        return result.IsSuccess ?
            File(result.Value.Content, result.Value.ContentType, result.Value.FileName) :
            result.ToProblem();
    }

    [HttpGet("stream/{id}")]
    public async Task<IActionResult> Stream([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await fileService.StreamAsync(id,userId, cancellationToken);

        return result.IsSuccess ?
            File(result.Value.Stream, result.Value.ContentType, result.Value.FileName,enableRangeProcessing:true) :
            result.ToProblem();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await fileService.DeleteAsync(id,userId, cancellationToken);
        return result.IsSuccess ?
            NoContent() :
            result.ToProblem();
    }
}
