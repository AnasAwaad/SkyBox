using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using SkyBox.API.Abstractions.Consts;
using SkyBox.API.Authentication;
using SkyBox.API.Helpers;
using System.Text;

namespace SkyBox.API.Services;


public class AuthService(UserManager<ApplicationUser> userManager,
    IJwtProvider jwtProvider,
    SignInManager<ApplicationUser> signInManager,
    ILogger<AuthService> logger,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor) : IAuthService
{
    public async Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        if (user.IsActive)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        var result = await signInManager.PasswordSignInAsync(user, password, false, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            // get roles
            var roles = await userManager.GetRolesAsync(user);

            // generate token 
            (string token, int expiresIn) = jwtProvider.GenerateToken(user,roles);

            await userManager.UpdateAsync(user);

            var response = new AuthResponse(user.Id, user.Email!, user.FullName, token, expiresIn);

            return Result.Success(response);
        }

        if (result.IsLockedOut)
            return Result.Failure<AuthResponse>(UserErrors.LockedUser);

        return Result.Failure<AuthResponse>(result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentials);
    }

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var emailExists = await userManager.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
            return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure<AuthResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }


        // Send confirmation email to user
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        logger.LogInformation("ConfirmationCode: {Code}", code);

        await SendConfirmationEmail(user, code);

        return Result.Success();
    }


    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        var user = await userManager.FindByIdAsync(request.UserId);

        if (user is null)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirmed);

        string decodedCode;
        try
        {
            decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await userManager.ConfirmEmailAsync(user, decodedCode);

        if (!result.Succeeded)
        {
            logger.LogInformation("Email confirmation failed for user {UserId}. Errors: {Errors}", request.UserId, string.Join(',', result.Errors.Select(e => e.Description)));
            var error = string.Join(',', result.Errors.Select(e => e.Description));
            return Result.Failure(new Error("User.InvalidCode", error, StatusCodes.Status400BadRequest));
        }

        await userManager.AddToRoleAsync(user, DefaultRoles.User);

        return Result.Success();
    }

    public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirmed);

        // Send confirmation email to user
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        logger.LogInformation("ConfirmationCode: {Code}", code);

        await SendConfirmationEmail(user, code);


        return Result.Success();
    }

    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        // Send confirmation email to user
        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        logger.LogInformation("Reset code: {Code}", code);

        await SendResetPasswordEmail(user, code);


        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCode);

        string decodedCode;
        IdentityResult result;
        try
        {
            decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result = await userManager.ResetPasswordAsync(user, decodedCode, request.NewPassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
        }

        if (!result.Succeeded)
        {
            logger.LogInformation("Password reset failed for user {UserId}. Errors: {Errors}", user.Id, string.Join(',', result.Errors.Select(e => e.Description)));
            var error = string.Join(',', result.Errors.Select(e => e.Description));
            return Result.Failure(new Error("User.InvalidCode", error, StatusCodes.Status400BadRequest));
        }

        return Result.Success();
    }

    private async Task SendConfirmationEmail(ApplicationUser user, string code)
    {
        var origin = httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            new Dictionary<string, string>
            {
                { "{{name}}", user.FullName },
                { "{{action_url}}", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
            });

        BackgroundJob.Enqueue(() => emailSender.SendEmailAsync(user.Email!, "Confirm your email", emailBody));

        await Task.CompletedTask;
    }

    private async Task SendResetPasswordEmail(ApplicationUser user, string code)
    {
        var origin = httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
            new Dictionary<string, string>
            {
                { "{{name}}", user.FullName },
                { "{{action_url}}", $"{origin}/auth/forgetPassword?email={user.Email}&code={code}" }
            });

        await emailSender.SendEmailAsync(user.Email!, "Change password", emailBody);

    }

}

