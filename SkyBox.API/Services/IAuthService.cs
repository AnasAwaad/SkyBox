namespace SkyBox.API.Services;

public interface IAuthService
{
    /// <summary>
    /// Authenticate a user using email and password.
    /// </summary>
    Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Register a new user account.
    /// </summary>
    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirm a user's email address.
    /// </summary>
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);

    /// <summary>
    /// Resend the email confirmation message.
    /// </summary>
    Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request);

    /// <summary>
    /// Send a password reset code to the user's email.
    /// </summary>
    Task<Result> SendResetPasswordCodeAsync(string email);

    /// <summary>
    /// Reset the user's password using a reset code.
    /// </summary>
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
}
