using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkyBox.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFolderShareTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FolderShares",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FolderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SharedWithUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Permission = table.Column<int>(type: "int", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolderShares_Folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4039BABA-A259-47DB-8FA5-D8CFF40BCBD9",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAI5kAtuwIBB0+BmqX8fyAthvaeYOeSspc9GeGmQeGeWLAsepC7WYrMGjToZfdnzAA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C1CB8E63-754C-4963-AFC0-B766711FAB0E",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFYuFfPc3T76xYj7z5VL8RqECaRRyhYcjyxUdJgAE7MnhzBGlbhopemrhMZK21jZYg==");

            migrationBuilder.CreateIndex(
                name: "IX_FolderShares_FolderId_SharedWithUserId",
                table: "FolderShares",
                columns: new[] { "FolderId", "SharedWithUserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FolderShares");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4039BABA-A259-47DB-8FA5-D8CFF40BCBD9",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMy7I6oO8/nUMHA7GjGeE3NZikheJ3jtehcfO7KzHQwYodCusWknbO5uQMeeRHY13A==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C1CB8E63-754C-4963-AFC0-B766711FAB0E",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEH7l1FAMy1SbTLQPKbvyq65OqWry9ujJNMDvwaUI1ptWahnssKmjZ7F1WqVkF1jezw==");
        }
    }
}
