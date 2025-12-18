using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.FileShares;

namespace SkyBox.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FileSharesController(IFileShareService fileShareService) : ControllerBase
{
    /// <summary>
    /// Share a file with another user.
    /// </summary>
    /// <remarks>
    /// Allows the file owner to share a file with another user
    /// and assign a specific permission (read or write).
    /// </remarks>
    /// <param name="fileId">The unique identifier of the file to share.</param>
    /// <param name="request">Sharing details including target user and permission.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <response code="200">File shared successfully.</response>
    /// <response code="400">Invalid request or file already shared.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to share this file.</response>
    /// <response code="404">File not found.</response>
    [HttpPost("{fileId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Share([FromRoute] Guid fileId, [FromBody] ShareFileRequest request, CancellationToken cancellationToken)
    {
        var result = await fileShareService.ShareAsync(fileId, User.GetUserId(), request, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    /// <summary>
    /// Revoke a file share.
    /// </summary>
    /// <remarks>
    /// Revokes access to a shared file for a specific user.
    /// Only the file owner can revoke sharing.
    /// </remarks>
    /// <param name="fileId">The file identifier.</param>
    /// <param name="userId">The user whose access will be revoked.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <response code="204">Share revoked successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    /// <response code="404">Share not found.</response>
    [HttpDelete("{fileId}/revoke/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Revoke([FromRoute] Guid fileId, [FromRoute] string userId, CancellationToken cancellationToken)
    {
        var result = await fileShareService.RevokeAsync(fileId, User.GetUserId(), userId, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    /// <summary>
    /// Update permission for a shared file.
    /// </summary>
    /// <remarks>
    /// Updates the permission level for a shared user.
    /// Only the file owner can modify permissions.
    /// </remarks>
    /// <param name="fileId">The file identifier.</param>
    /// <param name="userId">The user whose permission will be updated.</param>
    /// <param name="request">New permission details.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <response code="204">Permission updated successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    /// <response code="404">Share not found.</response>
    [HttpPut("{fileId}/permission/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePermission([FromRoute] Guid fileId, [FromRoute] string userId, [FromBody] UpdateSharePermissionRequest request, CancellationToken cancellationToken)
    {
        var result = await fileShareService.UpdatePermissionAsync(fileId, User.GetUserId(), userId, request.Permission, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
