using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkyBox.API.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerRelationShipWithFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folders_AspNetUsers_OwnerId",
                table: "Folders");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Folders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Files",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4039BABA-A259-47DB-8FA5-D8CFF40BCBD9",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIfzB2xSuChG/QvKFaGHnawaTbp33hLyJ8iV/D7Rws6iQSvBx96yexjmR2BMXRWKQw==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C1CB8E63-754C-4963-AFC0-B766711FAB0E",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFlmXtgBgJEZuywrtNz+Qu9Am3diguQ0PdfqDanwawdU+72UmmH44S7HF00elufZDQ==");

            migrationBuilder.CreateIndex(
                name: "IX_Files_OwnerId",
                table: "Files",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_AspNetUsers_OwnerId",
                table: "Files",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_AspNetUsers_OwnerId",
                table: "Folders",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_AspNetUsers_OwnerId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Folders_AspNetUsers_OwnerId",
                table: "Folders");

            migrationBuilder.DropIndex(
                name: "IX_Files_OwnerId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Files");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Folders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4039BABA-A259-47DB-8FA5-D8CFF40BCBD9",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMHL3N2Iaa6RmWvkhW+B4Xz3gnhPr6taHtlhIGH0XqbTI31xDG7kXru5bC6rfg6Wmw==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C1CB8E63-754C-4963-AFC0-B766711FAB0E",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAECKIIN/XwmsVg0yZ6mTSspIY15iErzLwjNkp0bnhUMIvWZqXlBpTBD2CpWjgkywJkA==");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_AspNetUsers_OwnerId",
                table: "Folders",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
