namespace SkyBox.API.Contracts.Auth;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
