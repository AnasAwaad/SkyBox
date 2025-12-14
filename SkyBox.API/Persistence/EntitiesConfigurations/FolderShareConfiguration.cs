using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkyBox.API.Persistence.EntitiesConfigurations;

public class FolderShareConfiguration : IEntityTypeConfiguration<FolderShare>
{
    public void Configure(EntityTypeBuilder<FolderShare> builder)
    {
        builder
            .HasOne(x => x.Folder)
            .WithMany()
            .HasForeignKey(x => x.FolderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => new { x.FolderId, x.SharedWithUserId })
            .IsUnique();

    }
}
