using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.Folder;
using SkyBox.API.Services;

namespace SkyBox.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize]
public class FoldersController(IFolderService folderService) : ControllerBase
{

    /// <summary>
    /// Create a new folder.
    /// </summary>
    /// <remarks>
    /// Creates a new folder for the current user.
    /// The folder can be created at the root level or inside a parent folder.
    /// </remarks>
    /// <param name="request">Folder creation request.</param>
    /// <response code="201">Folder created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpPost]
    [ProducesResponseType(typeof(FolderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateFolder([FromBody] FolderRequest request, CancellationToken cancellationToken)
    {
        var result = await folderService.CreateFolderAsync(request, User.GetUserId(), cancellationToken);
        return result.IsSuccess ?
            CreatedAtAction(nameof(GetFolderContent), new { id = result.Value.Id }, result.Value) :
            result.ToProblem();
    }

    /// <summary>
    /// Get root folder content for the current user.
    /// </summary>
    /// <remarks>
    /// Returns folders and files located at the user's root level.
    /// Supports pagination, searching, and sorting.
    /// </remarks>
    /// <param name="filters">Query filters for pagination, search, and sorting.</param>
    /// <response code="200">Returns paginated root folder content.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<FolderContentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRootContent([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var result = await folderService.GetFolderRootContentAsync(filters, User.GetUserId(), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Get content of a specific folder.
    /// </summary>
    /// <remarks>
    /// Returns subfolders and files inside the specified folder.
    /// User must be the owner or have the folder shared with them.
    /// </remarks>
    /// <param name="id">Folder unique identifier.</param>
    /// <param name="filters">Pagination, search and sorting filters.</param>
    /// <response code="200">Returns paginated folder content.</response>
    /// <response code="404">Folder not found.</response>
    /// <response code="403">User does not have access to the folder.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PaginatedList<FolderContentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]

    public async Task<IActionResult> GetFolderContent([FromRoute] Guid id,[FromQuery] RequestFilters filters ,CancellationToken cancellationToken)
    {
        var result = await folderService.GetFolderContentAsync(filters,id, User.GetUserId(),cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Rename an existing folder.
    /// </summary>
    /// <remarks>
    /// Updates the name of an existing folder.
    /// User must be the owner of the folder.
    /// </remarks>
    /// <param name="id">Folder unique identifier.</param>
    /// <param name="request">New folder name.</param>
    /// <response code="204">Folder renamed successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    /// <response code="404">Folder not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RenameFolder([FromRoute] Guid id, [FromBody] RenameFolderRequest request, CancellationToken cancellationToken)
    {
        var result = await folderService.RenameFolderAsync(id,request.Name, User.GetUserId(), cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }


    /// <summary>
    /// Toggle favorite status for a folder.
    /// </summary>
    /// <param name="id">Folder unique identifier.</param>
    /// <response code="204">Folder favorite status toggled successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to toggle favorite status to the folder.</response>
    /// <response code="404">Folder not found.</response>
    [HttpPut("{id}/favorite")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleFavorite([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await folderService.ToggleFavoriteStatusAsync(id, User.GetUserId(), cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
