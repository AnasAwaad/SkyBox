namespace SkyBox.API.Validators.Common;

public class UploadManyFilesRequestValidator : AbstractValidator<UploadManyFilesRequest>
{
    public UploadManyFilesRequestValidator()
    {
        RuleForEach(x=>x.Files)
            .SetValidator(new FileSizeValidator())
            .SetValidator(new BlockedSignatureValidator())
            .SetValidator(new FileNameValidator());
      
    }
}
