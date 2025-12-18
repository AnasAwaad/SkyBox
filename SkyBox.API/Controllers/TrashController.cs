using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.Files;
using SkyBox.API.Services;

namespace SkyBox.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TrashController(ITrashService trashService) : ControllerBase
{
    /// <summary>
    /// Get trashed files for the current user.
    /// </summary>
    /// <remarks>
    /// Returns a paginated list of files that were soft-deleted
    /// and are currently in the trash.
    /// </remarks>
    /// <param name="filters">Pagination, search and sorting filters.</param>
    /// <response code="200">Returns paginated trashed files.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TrashedFileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTrashFiles([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var result = await trashService.GetTrashFilesAsync(filters, cancellationToken);
        return Ok(result.Value);
    }

    /// <summary>
    /// Restore a file from the trash.
    /// </summary>
    /// <param name="id">Trashed file identifier.</param>
    /// <response code="204">File restored successfully.</response>
    /// <response code="404">File not found in trash.</response>
    [HttpPost("{id}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreFile([FromRoute] Guid id,CancellationToken cancellationToken)
    {
        var result = await trashService.RestoreAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    /// <summary>
    /// Permanently delete a file from the trash.
    /// </summary>
    /// <param name="id">Trashed file identifier.</param>
    /// <response code="204">File permanently deleted.</response>
    /// <response code="404">File not found.</response>
    [HttpDelete("{id}/permanent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePermanent(Guid id, CancellationToken cancellationToken)
    {
        var result = await trashService.PermanentlyDeleteAsync(id, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    /// <summary>
    /// Permanently delete all files in the trash.
    /// </summary>
    /// <remarks>
    /// This operation cannot be undone.
    /// </remarks>
    /// <response code="200">Trash emptied successfully.</response>
    [HttpDelete()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> EmptyTrash(CancellationToken cancellationToken)
    {
        var result = await trashService.EmptyTrashAsync(cancellationToken);

        return result.IsSuccess ? Ok(new {deletedCount = result.Value}) : result.ToProblem();
    }

}
