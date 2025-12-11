using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkyBox.API.Persistence.EntitiesConfigurations;

public class FileVersionConfiguration : IEntityTypeConfiguration<FileVersion>
{
    public void Configure(EntityTypeBuilder<FileVersion> builder)
    {
        builder.HasOne(x => x.File)
            .WithMany(x => x.Versions)
            .HasForeignKey(x => x.FileId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Property(x => x.FileName).HasMaxLength(250);
        builder.Property(x => x.StoredFileName).HasMaxLength(250);
        builder.Property(x => x.ContentType).HasMaxLength(50);
        builder.Property(x => x.FileExtension).HasMaxLength(10);
    }
}
