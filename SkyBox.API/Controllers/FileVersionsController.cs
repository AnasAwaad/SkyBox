using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.FileVersion;

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


}
