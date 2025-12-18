using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SkyBox.API.Contracts.SharedLink;

namespace SkyBox.API.Controllers;
[Route("api/[controller]")]
[ApiController]

public class SharesController(ISharedLinkService sharedLinkService) : ControllerBase
{
    /// <summary>
    /// Get all active shared links created by the current user.
    /// </summary>
    /// <param name="filters">Pagination, search and sorting filters.</param>
    /// <response code="200">Returns paginated list of shared links.</response>
    /// <response code="401">User is not authenticated.</response>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<SharedLinkResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyLinks([FromQuery] RequestFilters filters,CancellationToken cancellationToken)
    {
        var result = await sharedLinkService.GetMyLinksAsync(User.GetUserId(), filters, cancellationToken);

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new shared link for a file.
    /// </summary>
    /// <param name="fileId">Target file identifier.</param>
    /// <param name="request">Shared link configuration.</param>
    /// <response code="201">Shared link created successfully.</response>
    /// <response code="404">File not found.</response>
    [Authorize]
    [HttpPost("{fileId}")]
    [ProducesResponseType(typeof(SharedLinkResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateShareLink([FromRoute] Guid fileId, [FromBody] CreateSharedLinkRequest request,CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result =await sharedLinkService.CreateSharedLinkAsync(fileId, userId, request, cancellationToken);

        return result.IsSuccess ?
            CreatedAtAction(nameof(GetInfo), new { token = result.Value.Token }, result.Value) :
            result.ToProblem();
    }

    /// <summary>
    /// Get public information about a shared link.
    /// </summary>
    /// <param name="token">Shared link token.</param>
    /// <response code="200">Returns public file info.</response>
    /// <response code="404">Shared link not found or expired.</response>
    [AllowAnonymous]
    [HttpGet("info/{token}")]
    [EnableRateLimiting("SharedLinksPolicy")]
    public async Task<IActionResult> GetInfo(string token, CancellationToken cancellationToken)
    {
        var result = await sharedLinkService.GetInfoByTokenAsync(token, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Download a file using a shared link.
    /// </summary>
    /// <param name="token">File to access file.</param>
    /// <param name="password">Password for validation.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <response code="200">File downloaded successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have access to the file.</response>
    /// <response code="404">File not found.</response>
    [AllowAnonymous]
    [HttpGet("download/{token}")]
    [EnableRateLimiting("SharedLinksPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download([FromRoute] string token,[FromQuery] string? password, CancellationToken cancellationToken)
    {
        var result = await sharedLinkService.DownloadByTokenAsync(token,password, cancellationToken);

        return result.IsSuccess ?
            File(result.Value.Content, result.Value.ContentType, result.Value.FileName) :
            result.ToProblem();
    }

    /// <summary>
    /// Stream a file using a shared link.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("stream/{token}")]
    [EnableRateLimiting("SharedLinksPolicy")]
    public async Task<IActionResult> Stream(string token,[FromQuery] string? password, CancellationToken cancellationToken)
    {
        var result = await sharedLinkService.StreamByTokenAsync(token, password, cancellationToken);

        return result.IsSuccess ?
            File(result.Value.Stream, result.Value.ContentType,enableRangeProcessing:true) :
            result.ToProblem();
    }
    /// <summary>
    /// Delete a shared link.
    /// </summary>
    /// <param name="id">Shared link identifier.</param>
    /// <response code="204">Shared link deleted.</response>
    /// <response code="404">Shared link not found.</response>
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
