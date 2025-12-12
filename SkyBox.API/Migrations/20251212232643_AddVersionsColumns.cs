using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkyBox.API.Migrations
{
    /// <inheritdoc />
    public partial class AddVersionsColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileVersion_Files_FileId",
                table: "FileVersion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileVersion",
                table: "FileVersion");

            migrationBuilder.RenameTable(
                name: "FileVersion",
                newName: "FileVersions");

            migrationBuilder.RenameIndex(
                name: "IX_FileVersion_FileId",
                table: "FileVersions",
                newName: "IX_FileVersions_FileId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "FileVersions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FileVersions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileVersions",
                table: "FileVersions",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4039BABA-A259-47DB-8FA5-D8CFF40BCBD9",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIELs/B5flav9+c16E7K8+q9mQQgwHGH8EavdorSLwKzOSonkeujDbIQoj1GmjJyDQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C1CB8E63-754C-4963-AFC0-B766711FAB0E",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAtKG6JMIUrbha0eYH1zVUFS7q/g8tk3DJqoAAp7Lc3Zm1eK2M7k2xPwgYjNRvZuvw==");

            migrationBuilder.AddForeignKey(
                name: "FK_FileVersions_Files_FileId",
                table: "FileVersions",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileVersions_Files_FileId",
                table: "FileVersions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileVersions",
                table: "FileVersions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "FileVersions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FileVersions");

            migrationBuilder.RenameTable(
                name: "FileVersions",
                newName: "FileVersion");

            migrationBuilder.RenameIndex(
                name: "IX_FileVersions_FileId",
                table: "FileVersion",
                newName: "IX_FileVersion_FileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileVersion",
                table: "FileVersion",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4039BABA-A259-47DB-8FA5-D8CFF40BCBD9",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEE+Gj6GtLpUQX4QdgByG6BncP8VLgwNgAIq89FQHalbiO4nO/Ua4UZUqsjYaRjGqoQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C1CB8E63-754C-4963-AFC0-B766711FAB0E",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAQ7AU9YXLBuSuQJUPPt5nohYA45RaHz+fzYeNGmmu/hO2hQOY54L5SCmKgmwghFpg==");

            migrationBuilder.AddForeignKey(
                name: "FK_FileVersion_Files_FileId",
                table: "FileVersion",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
