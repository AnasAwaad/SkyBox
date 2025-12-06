using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Services;

namespace SkyBox.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TrashController(ITrashService trashService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTrashFiles([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var result = await trashService.GetTrashFilesAsync(filters, cancellationToken);
        return Ok(result.Value);
    }

    [HttpPost("{id}/restore")]
    public async Task<IActionResult> RestoreFile([FromRoute] Guid id,CancellationToken cancellationToken)
    {
        var result = await trashService.RestoreAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{id}/permanent")]
    public async Task<IActionResult> DeletePermanent(Guid id, CancellationToken cancellationToken)
    {
        var result = await trashService.PermanentlyDeleteAsync(id, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("trash")]
    public async Task<IActionResult> EmptyTrash(CancellationToken cancellationToken)
    {
        var result = await trashService.EmptyTrashAsync(cancellationToken);

        return result.IsSuccess ? Ok(new {deletedCount = result.Value}) : result.ToProblem();
    }

}
