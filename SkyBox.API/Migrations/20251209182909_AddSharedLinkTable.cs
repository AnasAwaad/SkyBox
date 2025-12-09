using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkyBox.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSharedLinkTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SharedLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 100, nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxDownloads = table.Column<int>(type: "int", nullable: true),
                    Downloads = table.Column<int>(type: "int", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Permission = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedLinks_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SharedLinks_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4039BABA-A259-47DB-8FA5-D8CFF40BCBD9",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAENHrhU7OzP2gi9FP8nERAXE5V1L52zxVXCjLf7ksRO+8QsrRj3Gx7xJ/o6VZh9CUjQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C1CB8E63-754C-4963-AFC0-B766711FAB0E",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJPYFQe26PX+UqP3hC2fKsIh2WX7a8Al0O0zYwAkauvlZCCEIgHcTdN22VTYzO2HtQ==");

            migrationBuilder.CreateIndex(
                name: "IX_SharedLinks_FileId",
                table: "SharedLinks",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedLinks_OwnerId",
                table: "SharedLinks",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedLinks_Token",
                table: "SharedLinks",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharedLinks");

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
        }
    }
}
