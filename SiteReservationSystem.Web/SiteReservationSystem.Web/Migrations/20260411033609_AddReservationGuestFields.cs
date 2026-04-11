using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteReservationSystem.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationGuestFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfGuests",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPets",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SpecialRequests",
                table: "Reservations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationID",
                keyValue: 1,
                columns: new[] { "NumberOfGuests", "NumberOfPets", "SpecialRequests" },
                values: new object[] { 0, 0, null });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationID",
                keyValue: 2,
                columns: new[] { "NumberOfGuests", "NumberOfPets", "SpecialRequests" },
                values: new object[] { 0, 0, null });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationID",
                keyValue: 3,
                columns: new[] { "NumberOfGuests", "NumberOfPets", "SpecialRequests" },
                values: new object[] { 0, 0, null });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationID",
                keyValue: 4,
                columns: new[] { "NumberOfGuests", "NumberOfPets", "SpecialRequests" },
                values: new object[] { 0, 0, null });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationID",
                keyValue: 5,
                columns: new[] { "NumberOfGuests", "NumberOfPets", "SpecialRequests" },
                values: new object[] { 0, 0, null });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "ReservationID",
                keyValue: 6,
                columns: new[] { "NumberOfGuests", "NumberOfPets", "SpecialRequests" },
                values: new object[] { 0, 0, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfGuests",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "NumberOfPets",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "SpecialRequests",
                table: "Reservations");
        }
    }
}
