using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.FolderShares;

namespace SkyBox.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class FolderSharesController(IFolderShareService folderShareService) : ControllerBase
{
    /// <summary>
    /// Share a folder with another user.
    /// </summary>
    /// <param name="folderId">Target folder identifier.</param>
    /// <param name="request">Folder share request details.</param>
    /// <response code="200">Folder shared successfully.</response>
    /// <response code="404">Folder not found.</response>
    /// <response code="409">Folder already shared with this user.</response>
    [HttpPost("{folderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Share([FromRoute] Guid folderId,[FromBody] ShareFolderRequest request,CancellationToken cancellationToken)
    {
        var result = await folderShareService.ShareAsync(folderId,User.GetUserId(),request,cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    /// <summary>
    /// Revoke folder access from a specific user.
    /// </summary>
    /// <param name="folderId">Folder identifier.</param>
    /// <param name="userId">User to revoke access from.</param>
    /// <response code="204">Folder share revoked successfully.</response>
    /// <response code="404">Folder share not found.</response>
    [HttpDelete("{folderId}/revoke/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Revoke([FromRoute] Guid folderId,[FromRoute] string userId,CancellationToken cancellationToken)
    {
        var result = await folderShareService.RevokeAsync(folderId,User.GetUserId(),userId,cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}