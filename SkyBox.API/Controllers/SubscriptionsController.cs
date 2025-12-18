using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkyBox.API.Contracts.Subscription;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SkyBox.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SubscriptionsController(ISubscriptionService subscriptionService) : ControllerBase
{

    /// <summary>
    /// Change a user's subscription plan.
    /// </summary>
    /// <remarks>
    /// This endpoint is restricted to administrators only.
    /// The plan cannot be downgraded below the user's current storage usage.
    /// </remarks>
    /// <param name="request">User and target subscription plan.</param>
    /// <response code="200">Subscription plan updated successfully.</response>
    /// <response code="400">Invalid plan change.</response>
    /// <response code="404">User not found.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpPost("change-plan")]
    [Authorize(Roles = DefaultRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ChangeUserSubscriptionPlan([FromBody] ChangeUserPlanRequest request,CancellationToken cancellationToken)
    {
        var result =await subscriptionService.ChangeUserPlan(request.UserId, request.Plan, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    /// <summary>
    /// Get the current user's subscription information.
    /// </summary>
    /// <remarks>
    /// Returns the active subscription plan along with storage usage
    /// and remaining available space.
    /// </remarks>
    /// <response code="200">Returns subscription information.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(MySubscriptionInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMySubscriptionInfo(CancellationToken cancellationToken)
    {
        var info = await subscriptionService.GetMySubscriptionInfo(User.GetUserId(), cancellationToken);

        return Ok(info);
    }

}
