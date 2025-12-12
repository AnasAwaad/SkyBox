using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.FileVersions;
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

    [Authorize]
    [HttpGet("{fileId}/versions/{versionId}/download")]
    public async Task<IActionResult> DownloadVersion(Guid fileId, Guid versionId, CancellationToken cancellationToken)
    {

        var result = await fileVersionService.DownloadVersionAsync(fileId, versionId, User.GetUserId(), cancellationToken);

        return result.IsSuccess ?
            File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : result.ToProblem();
    }

    [Authorize]
    [HttpDelete("{fileId}/versions/{versionId}")]
    public async Task<IActionResult> DeleteVersion(Guid fileId, Guid versionId, CancellationToken cancellationToken)
    {
        var result = await fileVersionService.DeleteVersionAsync(fileId, versionId, User.GetUserId(), cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [Authorize]
    [HttpPut("{fileId}/versions/{versionId}")]
    public async Task<IActionResult> UpdateVersionDescription(Guid fileId, Guid versionId,[FromBody] UpdateVersionDescriptionRequest request,CancellationToken cancellationToken)
    {
        var result = await fileVersionService.UpdateVersionDescriptionAsync(fileId, versionId, User.GetUserId(), request.Description, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

}
