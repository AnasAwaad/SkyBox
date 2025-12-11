using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkyBox.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFileVersionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileVersion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileVersion_Files_FileId",
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
                value: "AQAAAAIAAYagAAAAEE+Gj6GtLpUQX4QdgByG6BncP8VLgwNgAIq89FQHalbiO4nO/Ua4UZUqsjYaRjGqoQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C1CB8E63-754C-4963-AFC0-B766711FAB0E",
                columns: new[] { "PasswordHash", "SubscriptionPlan" },
                values: new object[] { "AQAAAAIAAYagAAAAEAQ7AU9YXLBuSuQJUPPt5nohYA45RaHz+fzYeNGmmu/hO2hQOY54L5SCmKgmwghFpg==", 2 });

            migrationBuilder.CreateIndex(
                name: "IX_FileVersion_FileId",
                table: "FileVersion",
                column: "FileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileVersion");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4039BABA-A259-47DB-8FA5-D8CFF40BCBD9",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOMYLbwlu6xdkX0I+QfSldIGByucVYR5ricmg5SSsD+xJ73SzBQNYy9HpuJYMzYRDQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C1CB8E63-754C-4963-AFC0-B766711FAB0E",
                columns: new[] { "PasswordHash", "SubscriptionPlan" },
                values: new object[] { "AQAAAAIAAYagAAAAEJLV9CzBiGqjD/IrFzy9XQ5wGcuuZ4LB8ldqjpCIpZLtz928QxRB8Yf9J6peortHEw==", 0 });
        }
    }
}
