using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.Folder;

namespace SkyBox.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FoldersController(IFolderService folderService) : ControllerBase
{

    [HttpPost]
    [Authorize(Roles = $"{DefaultRoles.User},{DefaultRoles.Admin}")]
    public async Task<IActionResult> CreateFolder([FromBody] FolderRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await folderService.CreateFolderAsync(request,userId, cancellationToken);
        return result.IsSuccess ?
            Created() :
            result.ToProblem();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{DefaultRoles.User},{DefaultRoles.Admin}")]
    public async Task<IActionResult> GetFolderContent([FromRoute] Guid id,[FromQuery] RequestFilters filters ,CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await folderService.GetFolderContentAsync(filters,id,userId,cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{DefaultRoles.User},{DefaultRoles.Admin}")]
    public async Task<IActionResult> RenameFolder([FromRoute] Guid Id, [FromBody] RenameFolderRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await folderService.RenameFolderAsync(Id,request.Name,userId, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
