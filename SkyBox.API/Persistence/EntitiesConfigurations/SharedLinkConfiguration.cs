using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkyBox.API.Persistence.EntitiesConfigurations;

public class SharedLinkConfiguration : IEntityTypeConfiguration<SharedLink>
{
    public void Configure(EntityTypeBuilder<SharedLink> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(200);

        // Token must be unique
        builder.HasIndex(x => x.Token)
            .IsUnique();

        builder.Property(x => x.Permission)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Relations

        builder.HasOne(x => x.File)
            .WithMany()
            .HasForeignKey(x => x.FileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Owner)
            .WithMany() 
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
