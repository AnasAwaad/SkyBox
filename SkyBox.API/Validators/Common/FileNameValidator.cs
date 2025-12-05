namespace SkyBox.API.Validators.Common;

public class FileNameValidator : AbstractValidator<IFormFile>
{

    public FileNameValidator()
    {
        // Validate file name format

        RuleFor(x => x.FileName)
            .Matches(@"^(?!\.)[A-Za-z0-9 _.-]+$")
            .WithMessage("Invalid file name format.")
            .When(x => x is not null);
    }
}
