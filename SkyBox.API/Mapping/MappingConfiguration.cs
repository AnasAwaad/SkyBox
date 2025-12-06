using SkyBox.API.Contracts.Folder;

namespace SkyBox.API.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Folder → FolderContentResponse
        TypeAdapterConfig<Folder, FolderChildrenResponse>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.IsFavorite, src => src.IsFavorite)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);

        // File → FolderContentResponse
        TypeAdapterConfig<UploadedFile, FolderFileChildrenResponse>.NewConfig()
            .Map(dest => dest.Name, src => src.FileName)
            .Map(dest => dest.CreatedAt, src => src.UploadedAt);
    }
}
