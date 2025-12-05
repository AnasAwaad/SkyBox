using SkyBox.API.Settings;

namespace SkyBox.API.Validators.Common;

public class FileSizeValidator : AbstractValidator<IFormFile>
{
    public FileSizeValidator()
    {
        RuleFor(x => x)
            .Must((request, context) => request.Length <= FileSettings.MaxFileSizeInBytes)
            .WithMessage($"File size must not exceed {FileSettings.MaxFileSizeInMB} MB.")//TODO: add error file messages
            .When(x => x is not null);
    }
}
