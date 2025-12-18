using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.FileVersions;

namespace SkyBox.API.Controllers;
[ApiController]
[Route("api/files")]
[Authorize]
[Produces("application/json")]
public class FileVersionsController(IFileVersionService fileVersionService) : ControllerBase
{
    /// <summary>
    /// Get all versions of a file.
    /// </summary>
    /// <remarks>
    /// Returns all non-deleted versions of a file ordered by creation date (latest first).
    /// Only the file owner is allowed to access version history.
    /// </remarks>
    /// <param name="fileId">The unique identifier of the file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns file versions.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    /// <response code="404">File not found.</response>
    [HttpGet("{fileId}/versions")]
    [ProducesResponseType(typeof(IEnumerable<FileVersionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<FileVersionResponse>>> GetVersions([FromRoute] Guid fileId, CancellationToken cancellationToken)
    {
        var result = await fileVersionService.GetAllVersionsAsync(fileId, User.GetUserId(), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Restore a specific file version.
    /// </summary>
    /// <remarks>
    /// Restores the selected version as the current file.
    /// The current version is backed up before restoring.
    /// Only the file owner is allowed to restore versions.
    /// </remarks>
    /// <param name="fileId">File identifier.</param>
    /// <param name="versionId">Version identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Version restored successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    /// <response code="404">File or version not found.</response>
    [HttpPost("{fileId}/versions/{versionId}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreVersion([FromRoute] Guid fileId, [FromRoute] Guid versionId, CancellationToken cancellationToken)
    {
        var result = await fileVersionService.RestoreVersionAsync(fileId, versionId, User.GetUserId(), cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    /// <summary>
    /// Download a specific file version.
    /// </summary>
    /// <remarks>
    /// Downloads the content of a selected file version.
    /// Only the file owner is allowed to download versions.
    /// </remarks>
    /// <param name="fileId">File identifier.</param>
    /// <param name="versionId">Version identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns file content.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    /// <response code="404">File or version not found.</response>
    [HttpGet("{fileId}/versions/{versionId}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadVersion([FromRoute] Guid fileId, [FromRoute] Guid versionId, CancellationToken cancellationToken)
    {
        var result = await fileVersionService.DownloadVersionAsync(fileId, versionId, User.GetUserId(), cancellationToken);

        return result.IsSuccess
            ? File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : result.ToProblem();
    }

    /// <summary>
    /// Delete a specific file version.
    /// </summary>
    /// <remarks>
    /// Soft deletes a file version without removing it from storage.
    /// Only the file owner can delete versions.
    /// </remarks>
    /// <param name="fileId">File identifier.</param>
    /// <param name="versionId">Version identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Version deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    /// <response code="404">File or version not found.</response>
    [HttpDelete("{fileId}/versions/{versionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVersion([FromRoute] Guid fileId, [FromRoute] Guid versionId, CancellationToken cancellationToken)
    {
        var result = await fileVersionService.DeleteVersionAsync(fileId, versionId, User.GetUserId(), cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    /// <summary>
    /// Update version description.
    /// </summary>
    /// <remarks>
    /// Updates the description of a specific file version.
    /// Only the file owner is allowed to modify version metadata.
    /// </remarks>
    /// <param name="fileId">File identifier.</param>
    /// <param name="versionId">Version identifier.</param>
    /// <param name="request">New version description.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Description updated successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    /// <response code="404">File or version not found.</response>
    [HttpPut("{fileId}/versions/{versionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVersionDescription([FromRoute] Guid fileId, [FromRoute] Guid versionId, [FromBody] UpdateVersionDescriptionRequest request, CancellationToken cancellationToken)
    {
        var result = await fileVersionService.UpdateVersionDescriptionAsync(fileId, versionId, User.GetUserId(), request.Description, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}

