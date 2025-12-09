using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.SharedLink;
using System.Threading.Tasks;

namespace SkyBox.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SharesController(ISharedLinkService sharedLinkService) : ControllerBase
{
    [HttpPost("{fileId}")]
    [Authorize(Roles = $"{DefaultRoles.Admin},{DefaultRoles.User}")]
    public async Task<IActionResult> CreateShareLink([FromRoute] Guid fileId, [FromBody] CreateSharedLinkRequest request,CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result =await sharedLinkService.CreateSharedLinkAsync(fileId, userId, request, cancellationToken);

        return result.IsSuccess ?
            Ok(result.Value) : // TODO: return the created resource location
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

}
