using SkyBox.API.Settings;

namespace SkyBox.API.Validators.Common;

public class BlockedSignatureValidator : AbstractValidator<IFormFile>
{
    public BlockedSignatureValidator()
    {
        RuleFor(x => x)
            .Must((request, context) =>
            {
                // check the file signature is not in the blocked list
                BinaryReader binary = new(request.OpenReadStream());
                var bytes = binary.ReadBytes(2);

                var fileSequenceHex = BitConverter.ToString(bytes);

                foreach (var signature in FileSettings.BlockedSignitures)
                    if (signature.Equals(fileSequenceHex, StringComparison.OrdinalIgnoreCase))
                        return false;

                return true;

            })
            .When(x => x is not null)
            .WithMessage("Not allowed file content");
    }
}
