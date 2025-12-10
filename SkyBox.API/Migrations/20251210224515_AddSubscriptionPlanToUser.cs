using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkyBox.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionPlanToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageQuotaBytes",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UsedStorageBytes",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionPlan",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4039BABA-A259-47DB-8FA5-D8CFF40BCBD9",
                columns: new[] { "PasswordHash", "SubscriptionPlan" },
                values: new object[] { "AQAAAAIAAYagAAAAEOMYLbwlu6xdkX0I+QfSldIGByucVYR5ricmg5SSsD+xJ73SzBQNYy9HpuJYMzYRDQ==", 0 });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C1CB8E63-754C-4963-AFC0-B766711FAB0E",
                columns: new[] { "PasswordHash", "SubscriptionPlan" },
                values: new object[] { "AQAAAAIAAYagAAAAEJLV9CzBiGqjD/IrFzy9XQ5wGcuuZ4LB8ldqjpCIpZLtz928QxRB8Yf9J6peortHEw==", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionPlan",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<long>(
                name: "StorageQuotaBytes",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UsedStorageBytes",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4039BABA-A259-47DB-8FA5-D8CFF40BCBD9",
                columns: new[] { "PasswordHash", "StorageQuotaBytes", "UsedStorageBytes" },
                values: new object[] { "AQAAAAIAAYagAAAAENHrhU7OzP2gi9FP8nERAXE5V1L52zxVXCjLf7ksRO+8QsrRj3Gx7xJ/o6VZh9CUjQ==", 10737418240L, 0L });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "C1CB8E63-754C-4963-AFC0-B766711FAB0E",
                columns: new[] { "PasswordHash", "StorageQuotaBytes", "UsedStorageBytes" },
                values: new object[] { "AQAAAAIAAYagAAAAEJPYFQe26PX+UqP3hC2fKsIh2WX7a8Al0O0zYwAkauvlZCCEIgHcTdN22VTYzO2HtQ==", 10737418240L, 0L });
        }
    }
}
