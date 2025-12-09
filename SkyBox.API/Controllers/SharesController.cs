using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.SharedLink;

namespace SkyBox.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SharesController(ISharedLinkService sharedLinkService) : ControllerBase
{
    [Authorize(Roles = $"{DefaultRoles.Admin},{DefaultRoles.User}")]
    [HttpGet]
    public async Task<IActionResult> GetMyLinks([FromQuery] RequestFilters filters,CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await sharedLinkService.GetMyLinksAsync(userId, filters, cancellationToken);

        return Ok(result.Value);
    }

    [HttpPost("{fileId}")]
    [Authorize(Roles = $"{DefaultRoles.Admin},{DefaultRoles.User}")]
    public async Task<IActionResult> CreateShareLink([FromRoute] Guid fileId, [FromBody] CreateSharedLinkRequest request,CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result =await sharedLinkService.CreateSharedLinkAsync(fileId, userId, request, cancellationToken);

        return result.IsSuccess ?
            CreatedAtAction(nameof(GetInfo), new { token = result.Value.Token }, result.Value) :
            result.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("info/{token}")]
    public async Task<IActionResult> GetInfo(string token, CancellationToken cancellationToken)
    {
        var result = await sharedLinkService.GetInfoByTokenAsync(token, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("download/{token}")]
    [AllowAnonymous]
    public async Task<IActionResult> Download([FromRoute] string token, CancellationToken cancellationToken)
    {
        var result = await sharedLinkService.DownloadByTokenAsync(token, cancellationToken);

        return result.IsSuccess ?
            File(result.Value.Content, result.Value.ContentType, result.Value.FileName) :
            result.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("stream/{token}")]
    public async Task<IActionResult> Stream(string token, CancellationToken cancellationToken)
    {
        var result = await sharedLinkService.StreamByTokenAsync(token, cancellationToken);

        return result.IsSuccess ?
            File(result.Value.Stream, result.Value.ContentType, result.Value.FileName,enableRangeProcessing:true) :
            result.ToProblem();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await sharedLinkService.DeleteAsync(id, userId, cancellationToken);

        return result.IsSuccess ?
            NoContent() :
            result.ToProblem();
    }

}
