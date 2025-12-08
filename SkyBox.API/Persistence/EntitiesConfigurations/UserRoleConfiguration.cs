using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkyBox.API.Abstractions.Consts;

namespace SkyBox.API.Persistence.EntitiesConfigurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {

        // Default Data
        builder.HasData(new IdentityUserRole<string>
        {
            UserId = DefaultUsers.AdminId,
            RoleId = DefaultRoles.AdminRoleId
        });

        builder.HasData(new IdentityUserRole<string>
        {
            UserId = DefaultUsers.UserId,
            RoleId = DefaultRoles.UserRoleId
        });

    }
}
