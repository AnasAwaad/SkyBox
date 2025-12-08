using SkyBox.API.Abstractions.Consts;

namespace SkyBox.API.Contracts.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
                .WithMessage("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit and one special character.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .Length(3, 150);

    }
}
