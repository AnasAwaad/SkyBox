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
    /// Change the subscription plan for a user by an admin.
    /// </summary>
    [HttpPost("change-plan")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> ChangeUserSubscriptionPlan([FromBody] ChangeUserPlanRequest request,CancellationToken cancellationToken)
    {
        var result =await subscriptionService.ChangeUserPlan(request.UserId, request.Plan, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    /// <summary>
    /// Get current user's subscription plan and storage usage.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMySubscriptionInfo(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var info = await subscriptionService.GetMySubscriptionInfo(userId, cancellationToken);

        return Ok(info);
    }

}
