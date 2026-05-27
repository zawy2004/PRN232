using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObjects.Migrations
{
    /// <inheritdoc />
    public partial class SeedMockData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AccountMembers",
                columns: new[] { "MemberID", "EmailAddress", "FullName", "MemberPassword", "MemberRole" },
                values: new object[,]
                {
                    { "admin", "admin@mystore.local", "System Admin", "admin123", 1 },
                    { "staff01", "staff01@mystore.local", "Store Staff", "staff123", 2 }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryID", "CategoryName" },
                values: new object[,]
                {
                    { 1, "Beverages" },
                    { 2, "Snacks" },
                    { 3, "Stationery" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductID", "CategoryID", "ProductName", "UnitPrice", "UnitsInStock" },
                values: new object[,]
                {
                    { 1, 1, "Green Tea", 1.99m, (short)120 },
                    { 2, 2, "Potato Chips", 2.49m, (short)85 },
                    { 3, 3, "Notebook A5", 3.25m, (short)200 },
                    { 4, 1, "Black Coffee", 4.10m, (short)60 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AccountMembers",
                keyColumn: "MemberID",
                keyValue: "admin");

            migrationBuilder.DeleteData(
                table: "AccountMembers",
                keyColumn: "MemberID",
                keyValue: "staff01");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 3);
        }
    }
}
