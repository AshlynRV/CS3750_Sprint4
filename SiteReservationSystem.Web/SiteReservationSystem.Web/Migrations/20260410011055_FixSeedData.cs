using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteReservationSystem.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceID",
                keyValue: 2,
                columns: new[] { "SubTotal", "TotalAmount" },
                values: new object[] { 225.00m, 250.00m });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 2,
                column: "Amount",
                value: 250.00m);

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationID",
                keyValue: 2,
                columns: new[] { "BaseAmount", "TotalAmount" },
                values: new object[] { 225.00m, 250.00m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "InvoiceID",
                keyValue: 2,
                columns: new[] { "SubTotal", "TotalAmount" },
                values: new object[] { 175.00m, 200.00m });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 2,
                column: "Amount",
                value: 200.00m);

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationID",
                keyValue: 2,
                columns: new[] { "BaseAmount", "TotalAmount" },
                values: new object[] { 175.00m, 200.00m });
        }
    }
}
