using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.FileVersion;
using System.Security.Claims;

namespace SkyBox.API.Controllers;
[Route("api/files")]
[ApiController]
public class FileVersionsController(IFileVersionService fileVersionService) : ControllerBase
{
    [HttpGet("{fileId}/versions")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<FileVersionResponse>>> GetVersions([FromRoute] Guid fileId,CancellationToken cancellationToken)
    {
        var result = await fileVersionService.GetAllVersionsAsync(fileId,User.GetUserId(),cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("{fileId}/versions/{versionId}/restore")]
    [Authorize]
    public async Task<IActionResult> RestoreVersion([FromRoute]Guid fileId,[FromRoute] Guid versionId, CancellationToken cancellationToken)
    {
        var result = await fileVersionService.RestoreVersionAsync(fileId, versionId, User.GetUserId(), cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

}
