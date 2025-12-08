using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SkyBox.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsersAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2C2DE682-7864-4119-A623-91FCE38F2418", "12947A3C-6DB7-4994-AD3E-6949BA8A963C", "Guest", "GUEST" },
                    { "4DB3DAEC-2623-409D-96C8-585DDFA644F4", "B6ED81F4-5DDA-422B-A14F-164D38415F6B", "User", "USER" },
                    { "61547292-2F3D-430D-ACD1-49756A3A6E9D", "BF84DE67-D1F8-4581-B740-878AD61E9AB4", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "FullName", "ImageUrl", "IsActive", "LastLoginAt", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "StorageQuotaBytes", "SuspendedAt", "SuspendedBy", "SuspendedReason", "TwoFactorEnabled", "UpdatedAt", "UsedStorageBytes", "UserName" },
                values: new object[,]
                {
                    { "4039BABA-A259-47DB-8FA5-D8CFF40BCBD9", 0, "948763CD-A96F-4ACB-8F88-910319C314A9", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user@gmail.com", true, "User", "", false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "USER@GMAIL.COM", "USER@GMAIL.COM", "AQAAAAIAAYagAAAAEMHL3N2Iaa6RmWvkhW+B4Xz3gnhPr6taHtlhIGH0XqbTI31xDG7kXru5bC6rfg6Wmw==", null, false, "B92157CE0AE54FB3A56AAFF531F6FF7A", 10737418240L, null, null, null, false, null, 0L, "user@gmail.com" },
                    { "C1CB8E63-754C-4963-AFC0-B766711FAB0E", 0, "ED2A8D98-1373-4A93-9440-FDF87961F6EE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@gmail.com", true, "Admin", "", false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAECKIIN/XwmsVg0yZ6mTSspIY15iErzLwjNkp0bnhUMIvWZqXlBpTBD2CpWjgkywJkA==", null, false, "F21B247ED97440039BBF0BB7F2EA7363", 10737418240L, null, null, null, false, null, 0L, "admin@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "4DB3DAEC-2623-409D-96C8-585DDFA644F4", "4039BABA-A259-47DB-8FA5-D8CFF40BCBD9" },
                    { "61547292-2F3D-430D-ACD1-49756A3A6E9D", "C1CB8E63-754C-4963-AFC0-B766711FAB0E" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2C2DE682-7864-4119-A623-91FCE38F2418");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4DB3DAEC-2623-409D-96C8-585DDFA644F4", "4039BABA-A259-47DB-8FA5-D8CFF40BCBD9" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "61547292-2F3D-430D-ACD1-49756A3A6E9D", "C1CB8E63-754C-4963-AFC0-B766711FAB0E" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4DB3DAEC-2623-409D-96C8-585DDFA644F4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "61547292-2F3D-430D-ACD1-49756A3A6E9D");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4039BABA-A259-47DB-8FA5-D8CFF40BCBD9");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C1CB8E63-754C-4963-AFC0-B766711FAB0E");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
