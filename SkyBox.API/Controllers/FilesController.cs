using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.Files;

namespace SkyBox.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize(Roles = $"{DefaultRoles.User},{DefaultRoles.Admin}")]
public class FilesController(IFileService fileService) : ControllerBase
{
    /// <summary>
    /// Upload a single file.
    /// </summary>
    /// <remarks>
    /// Uploads a file for the authenticated user.
    /// If a file with the same name already exists in the target folder,
    /// a new file version will be created instead of overwriting the file.
    /// </remarks>
    /// <param name="request">File upload request containing the file and optional folder ID.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <response code="201">File uploaded successfully.</response>
    /// <response code="400">Invalid file or request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to upload to the target folder.</response>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(FileUploadResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<FileUploadResponse>> UploadFile([FromForm] UploadFileRequest request, CancellationToken cancellationToken)
    {
        var result = await fileService.UploadAsync(request.File, User.GetUserId(), request.FolderId, cancellationToken);

        return result.IsSuccess ? CreatedAtAction(nameof(DownloadFile), new { id = result.Value.Id }, result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Upload multiple files.
    /// </summary>
    /// <remarks>
    /// Uploads multiple files in a single request.
    /// Each file is validated individually.
    /// If a file already exists, a new version is created.
    /// </remarks>
    /// <param name="request">Files upload request.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <response code="201">Files uploaded successfully.</response>
    /// <response code="400">Invalid files provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpPost("upload-many")]
    [ProducesResponseType(typeof(IEnumerable<FileUploadResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<FileUploadResponse>>> UploadManyFiles([FromForm] UploadManyFilesRequest request, CancellationToken cancellationToken)
    {
        var result = await fileService.UploadManyAsync(request.Files, User.GetUserId(), request.FolderId, cancellationToken);

        return result.IsSuccess ? CreatedAtAction(nameof(DownloadFile), null, result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Download a file.
    /// </summary>
    /// <remarks>
    /// Downloads the file content.
    /// The user must be the file owner or have access via sharing.
    /// </remarks>
    /// <param name="id">File unique identifier.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <response code="200">File downloaded successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have access to the file.</response>
    /// <response code="404">File not found.</response>
    [HttpGet("download/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadFile([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await fileService.DownloadAsync(id, User.GetUserId(), cancellationToken);

        return result.IsSuccess ? File(result.Value.Content, result.Value.ContentType, result.Value.FileName) : result.ToProblem();
    }

    /// <summary>
    /// Stream a file.
    /// </summary>
    /// <remarks>
    /// Streams the file content with range processing enabled.
    /// Useful for media files such as videos and audio.
    /// </remarks>
    /// <param name="id">File unique identifier.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <response code="200">File stream started.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have access to the file.</response>
    /// <response code="404">File not found.</response>
    [HttpGet("stream/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Stream([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await fileService.StreamAsync(id, User.GetUserId(), cancellationToken);

        return result.IsSuccess ? File(result.Value.Stream, result.Value.ContentType, result.Value.FileName, enableRangeProcessing: true) : result.ToProblem();
    }

    /// <summary>
    /// Toggle favorite status for a file.
    /// </summary>
    /// <param name="id">File unique identifier.</param>
    /// <response code="204">File favorite toggled successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to toggle favorite status to the file.</response>
    /// <response code="404">File not found.</response>
    [HttpPut("{id}/favorite")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleFavorite([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await fileService.ToggleFavoriteStatusAsync(id, User.GetUserId(), cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    /// <summary>
    /// Delete a file.
    /// </summary>
    /// <remarks>
    /// Performs a soft delete on the file.
    /// The file will no longer be accessible but can be restored if versioning is supported.
    /// Only the file owner can delete the file.
    /// </remarks>
    /// <param name="id">File unique identifier.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <response code="204">File deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to delete the file.</response>
    /// <response code="404">File not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFile([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await fileService.DeleteAsync(id, User.GetUserId(), cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
