using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SiteReservationSystem.Web.Migrations
{
    /// <inheritdoc />
    public partial class FinalSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoDStatus",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerID",
                keyValue: 1,
                column: "DoDStatus",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerID",
                keyValue: 2,
                column: "DoDStatus",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerID",
                keyValue: 3,
                column: "DoDStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerID",
                keyValue: 4,
                column: "DoDStatus",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerID",
                keyValue: 5,
                column: "DoDStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerID",
                keyValue: 6,
                column: "DoDStatus",
                value: 0);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "DateCreated", "Email", "IsActive", "LastLogin", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 13, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.doe@rvpark.com", true, null, "hashed_password_13", 1 },
                    { 14, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "bob.smith@rvpark.com", true, null, "hashed_password_14", 1 },
                    { 15, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "carol.white@rvpark.com", true, null, "hashed_password_15", 1 },
                    { 16, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "dan.locked@rvpark.com", false, null, "hashed_password_16", 1 }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeID", "AccessPermissions", "DateHired", "FirstName", "IsActive", "IsLockedOut", "LastName", "UserID" },
                values: new object[,]
                {
                    { 1, 29, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jane", true, false, "Doe", 13 },
                    { 2, 24, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bob", true, false, "Smith", 14 },
                    { 3, 16, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Carol", true, false, "White", 15 },
                    { 4, 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dan", false, true, "Locked", 16 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 16);

            migrationBuilder.DropColumn(
                name: "DoDStatus",
                table: "Customers");
        }
    }
}
