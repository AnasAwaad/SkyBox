using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.FileShares;

namespace SkyBox.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[Controller]")]
public class FileSharesController(IFileShareService fileShareService) : ControllerBase
{
    [HttpPost("{fileId}")]
    public async Task<IActionResult> Share([FromRoute] Guid fileId, [FromBody] ShareFileRequest request, CancellationToken cancellationToken)
    {
        var result = await fileShareService.ShareAsync(fileId, User.GetUserId(), request, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpGet("shared-with-me")]
    public async Task<IActionResult> SharedWithMe([FromQuery] int PageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await fileShareService.GetSharedWithMeAsync(User.GetUserId(), PageNumber, pageSize);
        return Ok(result.Value);
    }

    [HttpDelete("{fileId}/revoke/{userId}")]
    public async Task<IActionResult> Revoke([FromRoute] Guid fileId, [FromRoute] string userId, CancellationToken cancellationToken)
    {
        var result = await fileShareService.RevokeAsync(fileId, User.GetUserId(), userId, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{fileId}/permission/{userId}")]
    public async Task<IActionResult> UpdatePermission([FromRoute] Guid fileId, [FromRoute] string userId, [FromBody] UpdateSharePermissionRequest request, CancellationToken cancellationToken)
    {
        var result = await fileShareService.UpdatePermissionAsync(fileId, User.GetUserId(), userId, request.Permission,cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

}

