using SkyBox.API.Contracts.FileShares;
using SkyBox.API.Contracts.FileVersions;
using SkyBox.API.Contracts.Folder;
using SkyBox.API.Contracts.SharedLink;

namespace SkyBox.API.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        TypeAdapterConfig<Folder, FolderChildrenResponse>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.IsFavorite, src => src.IsFavorite)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);

        TypeAdapterConfig<UploadedFile, FolderFileChildrenResponse>.NewConfig()
            .Map(dest => dest.Name, src => src.FileName)
            .Map(dest => dest.CreatedAt, src => src.UploadedAt);


        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email);


        config.NewConfig<SharedLink, SharedLinkResponse>()
            .Map(dest => dest.FileName, src => src.File.FileName)
            .Map(dest => dest.FileSize, src => src.File.Size);


        config.NewConfig<FileVersion, FileVersionResponse>();

        config.NewConfig<UploadedFile, FileVersion>()
            .Map(dest => dest.FileId, src => src.Id)
            .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
            .Map(dest => dest.Description, src => $"Version on {DateTime.UtcNow:O}");

        config.NewConfig<FileShare, SharedWithMeResponse>()
            .Map(dest => dest.FileName, src => src.File.FileName)
            .Map(dest => dest.Size, src => src.File.Size)
            .Map(dest => dest.ContentType, src => src.File.ContentType)
            .Map(dest => dest.OwnerName, src => src.Owner.UserName!)
            .Map(dest => dest.SharedAt, src => src.CreatedAt);

    }
}
