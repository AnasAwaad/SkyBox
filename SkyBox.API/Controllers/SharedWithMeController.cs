using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.SharedWithMe;

namespace SkyBox.API.Controllers;
[Route("api/shared-with-me")]
[Authorize]

[ApiController]
public class SharedWithMeController(ISharedWithMeService sharedWithMeService) : ControllerBase
{
    /// <summary>
    /// Get all files and folders shared with the current user.
    /// </summary>
    /// <remarks>
    /// Returns a unified list of shared files and folders,
    /// including permission and owner information.
    /// Supports pagination, search and sorting.
    /// </remarks>
    /// <param name="filters">Pagination, search and sorting filters.</param>
    /// <response code="200">Returns paginated shared items.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<SharedWithMeItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSharedWithMe([FromQuery] RequestFilters filters,CancellationToken cancellationToken)
    {
        var result = await sharedWithMeService.GetSharedWithMeAsync(User.GetUserId(), filters, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
