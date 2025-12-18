using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.Favorites;

namespace SkyBox.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FavoritesController(IFavoriteService favoriteService) : ControllerBase
{
    ///<summary>
    /// Get all favorites files and folders for the current user.
    ///</summary>
    ///<remarks>
    /// Returns a unified, paginated list of user's favorite items (files and folders)
    /// </remarks>
    /// <param name="filters">Pagination, search and sorting filers</param>
    /// <response code="200">Returns paginated favorite items.</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<FavoriteItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetFavorites([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var result = await favoriteService.GetFavoritesAsync(User.GetUserId(), filters, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
