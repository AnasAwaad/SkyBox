using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkyBox.API.Abstractions.Consts;

namespace SkyBox.API.Persistence.EntitiesConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        // Default Data
        builder.HasData([
            new IdentityRole{
                Id = DefaultRoles.AdminRoleId,
                Name = DefaultRoles.Admin,
                NormalizedName = DefaultRoles.Admin.ToUpper(),
                ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp,
            },
            new IdentityRole{
                Id = DefaultRoles.UserRoleId,
                Name = DefaultRoles.User,
                NormalizedName = DefaultRoles.User.ToUpper(),
                ConcurrencyStamp = DefaultRoles.UserRoleConcurrencyStamp,
            },
            new IdentityRole{
                Id = DefaultRoles.GuestRoleId,
                Name = DefaultRoles.Guest,
                NormalizedName = DefaultRoles.Guest.ToUpper(),
                ConcurrencyStamp = DefaultRoles.GuestRoleConcurrencyStamp,
            }
        ]);

    }
}