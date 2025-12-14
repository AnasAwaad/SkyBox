using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.FolderShares;

namespace SkyBox.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FolderSharesController(IFolderShareService folderShareService) : ControllerBase
{
    [HttpPost("{folderId}")]
    public async Task<IActionResult> Share([FromRoute] Guid folderId, [FromBody] ShareFolderRequest request)
    {
        var result = await folderShareService.ShareAsync(folderId, User.GetUserId(), request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpDelete("{folderId}/revoke/{userId}")]
    public async Task<IActionResult> Revoke([FromRoute] Guid folderId, [FromRoute] string userId)
    {
        var result = await folderShareService.RevokeAsync(folderId, User.GetUserId(), userId);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
