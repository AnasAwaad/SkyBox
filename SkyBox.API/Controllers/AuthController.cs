using Microsoft.AspNetCore.Mvc;
namespace SkyBox.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Authenticate a user and generate an access token.
    /// </summary>
    /// <param name="request">User login credentials.</param>
    /// <response code="200">Login successful.</response>
    /// <response code="401">Invalid email or password.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request.Email, request.Password, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Register a new user account.
    /// </summary>
    /// <remarks>
    /// Sends an email confirmation link after successful registration.
    /// </remarks>
    /// <param name="request">User registration details.</param>
    /// <response code="200">Registration successful.</response>
    /// <response code="400">Invalid registration data.</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    /// <summary>
    /// Confirm a user's email address.
    /// </summary>
    /// <param name="request">Email confirmation data.</param>
    /// <response code="200">Email confirmed successfully.</response>
    /// <response code="400">Invalid or expired confirmation token.</response>
    [HttpPost("confirm-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        var result = await authService.ConfirmEmailAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    /// <summary>
    /// Send a password reset code to the user's email.
    /// </summary>
    /// <param name="request">Email address for password reset.</param>
    /// <response code="200">Reset password email sent.</response>
    [HttpPost("forget-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
    {
        await authService.SendResetPasswordCodeAsync(request.Email);

        return Ok();
    }

    /// <summary>
    /// Reset a user's password using a reset code.
    /// </summary>
    /// <param name="request">Password reset details.</param>
    /// <response code="200">Password reset successfully.</response>
    /// <response code="400">Invalid reset code or password.</response>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await authService.ResetPasswordAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    /// <summary>
    /// Resend the email confirmation message.
    /// </summary>
    /// <param name="request">Resend confirmation email request.</param>
    /// <response code="200">Confirmation email resent.</response>
    /// <response code="400">Email already confirmed or invalid.</response>
    [HttpPost("resend-confirmation-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest request)
    {
        var result = await authService.ResendConfirmationEmailAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

}
