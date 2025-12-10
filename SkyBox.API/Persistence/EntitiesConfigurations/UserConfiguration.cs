using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkyBox.API.Abstractions.Consts;

namespace SkyBox.API.Persistence.EntitiesConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(p => p.FullName).HasMaxLength(100);

        var passwordHasher = new PasswordHasher<ApplicationUser>();

        // Default data
        builder.HasData(new ApplicationUser
        {
            Id = DefaultUsers.AdminId,
            FullName = "Admin",
            UserName = DefaultUsers.AdminEmail,
            NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
            Email = DefaultUsers.AdminEmail,
            NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
            ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
            SecurityStamp = DefaultUsers.AdminSecurityStamp,
            EmailConfirmed = true,
            PasswordHash = passwordHasher.HashPassword(null!, DefaultUsers.AdminPassword),
            SubscriptionPlan = SubscriptionPlan.Business
        });

        builder.HasData(new ApplicationUser
        {
            Id = DefaultUsers.UserId,
            FullName = "User",
            UserName = DefaultUsers.UserEmail,
            NormalizedUserName = DefaultUsers.UserEmail.ToUpper(),
            Email = DefaultUsers.UserEmail,
            NormalizedEmail = DefaultUsers.UserEmail.ToUpper(),
            ConcurrencyStamp = DefaultUsers.UserConcurrencyStamp,
            SecurityStamp = DefaultUsers.UserSecurityStamp,
            EmailConfirmed = true,
            PasswordHash = passwordHasher.HashPassword(null!, DefaultUsers.UserPassword)
        });

    }
}
