using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SiteReservationSystem.Web.Migrations
{
    /// <inheritdoc />
    public partial class SeedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SiteTypes",
                columns: new[] { "SiteTypeID", "Description", "IsActive", "TypeName" },
                values: new object[,]
                {
                    { 1, "Standard trailer sites for vehicles 1-45 feet", true, "Trailers (1-45)" },
                    { 2, "Walk-in trailer sites with linens provided", true, "Walk-in Trailers (11B, 12B)" },
                    { 3, "Dry storage sites for RV parking", true, "Dry Storage" },
                    { 4, "Tent camping site near Dog Park", true, "Tent Site" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "DateCreated", "Email", "IsActive", "LastLogin", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ashlyn.arave@example.com", true, null, "hashed_password_1", 2 },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "kelise.bridge@example.com", true, null, "hashed_password_2", 2 },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "zachary.chamberlain@example.com", true, null, "hashed_password_3", 2 },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "tyler.fleischel@example.com", true, null, "hashed_password_4", 2 },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "nicole.gaddis@example.com", true, null, "hashed_password_5", 2 },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "luke.peterson@example.com", true, null, "hashed_password_6", 2 },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.smith@military.com", true, null, "hashed_password_7", 0 },
                    { 8, new DateTime(2024, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "sarah.johnson@military.com", true, null, "hashed_password_8", 0 },
                    { 9, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "mike.williams@military.com", true, null, "hashed_password_9", 0 },
                    { 10, new DateTime(2025, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "emily.davis@military.com", true, null, "hashed_password_10", 0 },
                    { 11, new DateTime(2024, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "david.martinez@military.com", true, null, "hashed_password_11", 0 },
                    { 12, new DateTime(2025, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "lisa.anderson@military.com", true, null, "hashed_password_12", 0 }
                });

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "AdminID", "DateCreated", "FirstName", "LastName", "UserID" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ashlyn", "Arave", 1 },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kelise", "Bridge", 2 },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Zachary", "Chamberlain", 3 },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tyler", "Fleischel", 4 },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nicole", "Gaddis", 5 },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Luke", "Peterson", 6 }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerID", "DateCreated", "FirstName", "LastName", "MilitaryAffiliation", "PhoneNumber", "UserID" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "John", "Smith", 0, "555-0101", 7 },
                    { 2, new DateTime(2024, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sarah", "Johnson", 1, "555-0102", 8 },
                    { 3, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mike", "Williams", 2, "555-0103", 9 },
                    { 4, new DateTime(2025, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Emily", "Davis", 4, "555-0104", 10 },
                    { 5, new DateTime(2024, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "David", "Martinez", 3, "555-0105", 11 },
                    { 6, new DateTime(2025, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lisa", "Anderson", 5, "555-0106", 12 }
                });

            migrationBuilder.InsertData(
                table: "SiteTypePricings",
                columns: new[] { "PricingID", "BasePrice", "Description", "EndDate", "SiteTypeID", "StartDate" },
                values: new object[,]
                {
                    { 1, 25.00m, "Current rate for standard trailers", null, 1, new DateTime(2024, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 30.00m, "Current rate for walk-in trailers", null, 2, new DateTime(2024, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 5.00m, "Current daily rate for dry storage (also $30/week, $36/month)", null, 3, new DateTime(2024, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 17.00m, "Current rate for tent site", null, 4, new DateTime(2024, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Sites",
                columns: new[] { "SiteID", "IsActive", "MaxLengthFeet", "Notes", "SiteNumber", "SiteTypeID" },
                values: new object[,]
                {
                    { 1, true, 40, null, "2", 1 },
                    { 2, true, 40, null, "3", 1 },
                    { 3, true, 40, null, "4", 1 },
                    { 4, true, 40, null, "5", 1 },
                    { 5, true, 40, null, "6", 1 },
                    { 6, true, 40, null, "7", 1 },
                    { 7, true, 40, null, "8", 1 },
                    { 8, true, 40, null, "9", 1 },
                    { 9, true, 40, null, "10", 1 },
                    { 10, true, 40, null, "11", 1 },
                    { 11, true, 40, null, "12", 1 },
                    { 12, true, 40, null, "13", 1 },
                    { 13, true, 40, null, "14", 1 },
                    { 14, true, 45, null, "17", 1 },
                    { 15, true, 45, null, "18", 1 },
                    { 16, true, 45, null, "20", 1 },
                    { 17, true, 45, null, "22", 1 },
                    { 18, true, 45, null, "23", 1 },
                    { 19, true, 45, null, "24", 1 },
                    { 20, true, 45, null, "25", 1 },
                    { 21, true, 45, null, "26", 1 },
                    { 22, true, 45, null, "27", 1 },
                    { 23, true, 45, null, "28", 1 },
                    { 24, true, 45, null, "29", 1 },
                    { 25, true, 45, null, "30", 1 },
                    { 26, true, 45, null, "31", 1 },
                    { 27, true, 65, null, "32", 1 },
                    { 28, true, 65, null, "33", 1 },
                    { 29, true, 65, null, "34", 1 },
                    { 30, true, 65, null, "35", 1 },
                    { 31, true, 65, null, "36", 1 },
                    { 32, true, 65, null, "37", 1 },
                    { 33, true, 65, null, "38", 1 },
                    { 34, true, 65, null, "39", 1 },
                    { 35, true, 65, null, "40", 1 },
                    { 36, true, 65, null, "41", 1 },
                    { 37, true, 65, null, "42", 1 },
                    { 38, true, 65, null, "43", 1 },
                    { 39, true, 65, null, "44", 1 },
                    { 40, true, 65, null, "45", 1 },
                    { 41, true, 55, "Exception - 55 feet", "1", 1 },
                    { 42, true, 55, "Exception - 55 feet", "19", 1 },
                    { 43, true, 55, "Exception - 55 feet", "21", 1 },
                    { 44, true, 30, "Walk-in trailer with linens", "11B", 2 },
                    { 45, true, 30, "Walk-in trailer with linens", "12B", 2 },
                    { 46, true, 65, "Dry storage site", "A", 3 },
                    { 47, true, 65, "Dry storage site", "B", 3 },
                    { 48, true, 65, "Dry storage site", "C", 3 },
                    { 49, true, 65, "Dry storage site", "D", 3 },
                    { 50, true, 0, "Tent site near Dog Park", "TENT-1", 4 }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "ReservationID", "ActualCheckInTime", "ActualCheckOutTime", "BalanceDue", "BaseAmount", "CustomerID", "DateCreated", "EndDate", "LastUpdated", "Notes", "ReservationStatusID", "ScheduledCheckInTime", "ScheduledCheckOutTime", "SiteID", "StartDate", "TotalAmount", "TrailerLengthFeet" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 2, 19, 13, 15, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 22, 11, 45, 0, 0, DateTimeKind.Unspecified), 0m, 75.00m, 1, new DateTime(2025, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 3, new DateTime(2025, 2, 19, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 22, 12, 0, 0, 0, DateTimeKind.Unspecified), 14, new DateTime(2025, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 75.00m, 42 },
                    { 2, new DateTime(2025, 2, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), null, 0m, 175.00m, 2, new DateTime(2025, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, new DateTime(2025, 2, 27, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 8, 12, 0, 0, 0, DateTimeKind.Unspecified), 27, new DateTime(2025, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 200.00m, 60 },
                    { 3, null, null, 0m, 350.00m, 3, new DateTime(2025, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, new DateTime(2025, 3, 8, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 22, 12, 0, 0, 0, DateTimeKind.Unspecified), 20, new DateTime(2025, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 350.00m, 38 },
                    { 4, null, null, 0m, 100.00m, 4, new DateTime(2025, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cancelled due to change in travel plans", 4, new DateTime(2025, 3, 11, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 15, 12, 0, 0, 0, DateTimeKind.Unspecified), 5, new DateTime(2025, 3, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 110.00m, 35 },
                    { 5, null, null, 0m, 90.00m, 5, new DateTime(2025, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, new DateTime(2025, 3, 4, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 7, 12, 0, 0, 0, DateTimeKind.Unspecified), 44, new DateTime(2025, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 105.00m, null },
                    { 6, null, null, 0m, 34.00m, 6, new DateTime(2025, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, new DateTime(2025, 3, 2, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 4, 12, 0, 0, 0, DateTimeKind.Unspecified), 50, new DateTime(2025, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 34.00m, null }
                });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "InvoiceID", "CustomerID", "DatePaid", "DueDate", "InvoiceDate", "IsPaid", "Notes", "ReservationID", "SubTotal", "TotalAmount", "TotalFees" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 1, 75.00m, 75.00m, 0m },
                    { 2, 2, new DateTime(2025, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 2, 175.00m, 200.00m, 25.00m },
                    { 3, 3, new DateTime(2025, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 3, 350.00m, 350.00m, 0m },
                    { 4, 4, new DateTime(2025, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 4, 100.00m, 110.00m, 10.00m },
                    { 5, 5, new DateTime(2025, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 5, 90.00m, 105.00m, 15.00m },
                    { 6, 6, new DateTime(2025, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 6, 34.00m, 34.00m, 0m }
                });

            migrationBuilder.InsertData(
                table: "ReservationFees",
                columns: new[] { "ReservationFeeID", "Amount", "DateApplied", "FeeID", "Notes", "ReservationID" },
                values: new object[,]
                {
                    { 1, 25.00m, new DateTime(2025, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Requested late checkout until 3:00 PM", 2 },
                    { 2, 10.00m, new DateTime(2025, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Cancelled more than 72 hours before arrival", 4 },
                    { 3, 15.00m, new DateTime(2025, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Early check-in requested for 8:00 AM", 5 }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentID", "Amount", "InvoiceID", "IsRefund", "Notes", "PaymentDate", "PaymentMethodID", "PaymentStatus", "ProcessedByEmployeeID", "StripeTransactionID" },
                values: new object[,]
                {
                    { 1, 75.00m, 1, false, null, new DateTime(2025, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Completed", null, "txn_1ABC123" },
                    { 2, 200.00m, 2, false, null, new DateTime(2025, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Completed", null, "txn_2DEF456" },
                    { 3, 350.00m, 3, false, null, new DateTime(2025, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Completed", null, "txn_3GHI789" },
                    { 4, 110.00m, 4, false, null, new DateTime(2025, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Completed", null, "txn_4JKL012" },
                    { 5, -100.00m, 4, true, "Refund issued: $100.00 (Original $110.00 - $10.00 cancellation fee)", new DateTime(2025, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Refunded", null, "txn_4JKL012_refund" },
                    { 6, 105.00m, 5, false, null, new DateTime(2025, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Completed", null, null },
                    { 7, 34.00m, 6, false, null, new DateTime(2025, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Completed", null, "txn_6MNO345" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "AdminID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "AdminID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "AdminID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "AdminID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "AdminID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "AdminID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ReservationFees",
                keyColumn: "ReservationFeeID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ReservationFees",
                keyColumn: "ReservationFeeID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ReservationFees",
                keyColumn: "ReservationFeeID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SiteTypePricings",
                keyColumn: "PricingID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SiteTypePricings",
                keyColumn: "PricingID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SiteTypePricings",
                keyColumn: "PricingID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SiteTypePricings",
                keyColumn: "PricingID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "InvoiceID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "InvoiceID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "InvoiceID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "InvoiceID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "InvoiceID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "InvoiceID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SiteTypes",
                keyColumn: "SiteTypeID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "ReservationID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "ReservationID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "ReservationID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "ReservationID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "ReservationID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "ReservationID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Sites",
                keyColumn: "SiteID",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "SiteTypes",
                keyColumn: "SiteTypeID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SiteTypes",
                keyColumn: "SiteTypeID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SiteTypes",
                keyColumn: "SiteTypeID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 12);
        }
    }
}
