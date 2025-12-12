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

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileVersions",
                table: "FileVersions",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4039BABA-A259-47DB-8FA5-D8CFF40BCBD9",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIHql9OY+1mN37EmBY0HcRTXjc2W5mcbYxmshHl4+gQgfvV/R/QuU5Ur/HuejUaOyg==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C1CB8E63-754C-4963-AFC0-B766711FAB0E",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMe0UUMe9PpyDFANV1PILq+YuB4zrIsdtszvTOSJ16zeezO644I4Bw68JgLO2+N0Yg==");

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
