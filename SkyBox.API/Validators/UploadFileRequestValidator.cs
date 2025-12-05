using SkyBox.API.Validators.Common;

namespace SkyBox.API.Validators;

public class UploadFileRequestValidator : AbstractValidator<UploadFileRequest>
{
    public UploadFileRequestValidator()
    {
        //RuleFor(x => x.File)
        //    .SetValidator(new FileSizeValidator())
        //    .SetValidator(new BlockedSignatureValidator())
        //    .SetValidator(new FileNameValidator());

    }


}
