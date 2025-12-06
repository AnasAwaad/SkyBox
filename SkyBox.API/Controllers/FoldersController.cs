using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.Folder;

namespace SkyBox.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FoldersController(IFolderService folderService) : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> CreateFolder([FromBody] FolderRequest request, CancellationToken cancellationToken)
    {
        var result = await folderService.CreateFolderAsync(request, cancellationToken);
        return result.IsSuccess ?
            Created() :
            result.ToProblem();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFolderContent([FromRoute] Guid id,[FromQuery] RequestFilters filters ,CancellationToken cancellationToken)
    {
        var result = await folderService.GetFolderContentAsync(id,filters,cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> RenameFolder([FromRoute] Guid Id, [FromBody] RenameFolderRequest request, CancellationToken cancellationToken)
    {
        var result = await folderService.RenameFolderAsync(Id,request.Name, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
