using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkyBox.API.Persistence.EntitiesConfigurations;

public class FileShareConfiguration : IEntityTypeConfiguration<FileShare>
{
    public void Configure(EntityTypeBuilder<FileShare> builder)
    {
        builder
            .HasOne(x => x.File)
            .WithMany()
            .HasForeignKey(x => x.FileId);

        builder
            .HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder 
            .HasOne(x => x.SharedWithUser)
            .WithMany()
            .HasForeignKey(x => x.SharedWithUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => new { x.FileId, x.SharedWithUserId })
            .IsUnique();
    }
}
