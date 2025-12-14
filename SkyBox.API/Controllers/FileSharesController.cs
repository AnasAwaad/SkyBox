using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.FileShares;

namespace SkyBox.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[Controller]")]
public class FileSharesController(IFileShareService fileShareService) : ControllerBase
{
    [HttpPost("{fileId}")]
    public async Task<IActionResult> Share([FromRoute] Guid fileId,[FromBody] ShareFileRequest request)
    {
        var result = await fileShareService.ShareAsync(fileId, User.GetUserId(), request);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpGet("shared-with-me")]
    public async Task<IActionResult> SharedWithMe()
    {
        var result = await fileShareService.GetSharedWithMeAsync(User.GetUserId());
        return Ok(result.Value);
    }

    [HttpDelete("{fileId}/revoke/{userId}")]
    public async Task<IActionResult> Revoke([FromRoute] Guid fileId, [FromRoute] string userId)
    {
        var result = await fileShareService.RevokeAsync(fileId, User.GetUserId(), userId);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}

