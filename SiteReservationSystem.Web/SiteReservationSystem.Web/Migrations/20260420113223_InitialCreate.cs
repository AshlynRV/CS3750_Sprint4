using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SiteReservationSystem.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fees",
                columns: table => new
                {
                    FeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DefaultAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fees", x => x.FeeID);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    PaymentMethodID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MethodName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequiresOnlineProcessing = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.PaymentMethodID);
                });

            migrationBuilder.CreateTable(
                name: "ReservationStatuses",
                columns: table => new
                {
                    ReservationStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationStatuses", x => x.ReservationStatusID);
                });

            migrationBuilder.CreateTable(
                name: "SiteTypes",
                columns: table => new
                {
                    SiteTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteTypes", x => x.SiteTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    SiteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SiteTypeID = table.Column<int>(type: "int", nullable: false),
                    MaxLengthFeet = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.SiteID);
                    table.ForeignKey(
                        name: "FK_Sites_SiteTypes_SiteTypeID",
                        column: x => x.SiteTypeID,
                        principalTable: "SiteTypes",
                        principalColumn: "SiteTypeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SiteTypePricings",
                columns: table => new
                {
                    PricingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteTypeID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BasePrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteTypePricings", x => x.PricingID);
                    table.ForeignKey(
                        name: "FK_SiteTypePricings_SiteTypes_SiteTypeID",
                        column: x => x.SiteTypeID,
                        principalTable: "SiteTypes",
                        principalColumn: "SiteTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminID);
                    table.ForeignKey(
                        name: "FK_Admins_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MilitaryAffiliation = table.Column<int>(type: "int", nullable: false),
                    DoDStatus = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerID);
                    table.ForeignKey(
                        name: "FK_Customers_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccessPermissions = table.Column<int>(type: "int", nullable: false),
                    IsLockedOut = table.Column<bool>(type: "bit", nullable: false),
                    DateHired = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeID);
                    table.ForeignKey(
                        name: "FK_Employees_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SitePhotos",
                columns: table => new
                {
                    PhotoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    PhotoURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    DateUploaded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitePhotos", x => x.PhotoID);
                    table.ForeignKey(
                        name: "FK_SitePhotos_Sites_SiteID",
                        column: x => x.SiteID,
                        principalTable: "Sites",
                        principalColumn: "SiteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ReservationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    ReservationStatusID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrailerLengthFeet = table.Column<int>(type: "int", nullable: true),
                    BaseAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BalanceDue = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ScheduledCheckInTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualCheckInTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduledCheckOutTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualCheckOutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NumberOfGuests = table.Column<int>(type: "int", nullable: false),
                    NumberOfPets = table.Column<int>(type: "int", nullable: false),
                    SpecialRequests = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ReservationID);
                    table.ForeignKey(
                        name: "FK_Reservations_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservations_ReservationStatuses_ReservationStatusID",
                        column: x => x.ReservationStatusID,
                        principalTable: "ReservationStatuses",
                        principalColumn: "ReservationStatusID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservations_Sites_SiteID",
                        column: x => x.SiteID,
                        principalTable: "Sites",
                        principalColumn: "SiteID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationID = table.Column<int>(type: "int", nullable: false),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalFees = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    DatePaid = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceID);
                    table.ForeignKey(
                        name: "FK_Invoices_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_Reservations_ReservationID",
                        column: x => x.ReservationID,
                        principalTable: "Reservations",
                        principalColumn: "ReservationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservationFees",
                columns: table => new
                {
                    ReservationFeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationID = table.Column<int>(type: "int", nullable: false),
                    FeeID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DateApplied = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationFees", x => x.ReservationFeeID);
                    table.ForeignKey(
                        name: "FK_ReservationFees_Fees_FeeID",
                        column: x => x.FeeID,
                        principalTable: "Fees",
                        principalColumn: "FeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationFees_Reservations_ReservationID",
                        column: x => x.ReservationID,
                        principalTable: "Reservations",
                        principalColumn: "ReservationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceID = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StripeTransactionID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProcessedByEmployeeID = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsRefund = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_Payments_Employees_ProcessedByEmployeeID",
                        column: x => x.ProcessedByEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Payments_Invoices_InvoiceID",
                        column: x => x.InvoiceID,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentMethods_PaymentMethodID",
                        column: x => x.PaymentMethodID,
                        principalTable: "PaymentMethods",
                        principalColumn: "PaymentMethodID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Fees",
                columns: new[] { "FeeID", "DefaultAmount", "Description", "FeeName", "IsActive" },
                values: new object[,]
                {
                    { 1, 25.00m, "Fee for checking out after designated time", "Late Checkout", true },
                    { 2, 15.00m, "Fee for checking in before standard time", "Early Check-in", true },
                    { 3, 10.00m, "Standard cancellation fee", "Cancellation Fee", true }
                });

            migrationBuilder.InsertData(
                table: "PaymentMethods",
                columns: new[] { "PaymentMethodID", "Description", "IsActive", "MethodName", "RequiresOnlineProcessing" },
                values: new object[,]
                {
                    { 1, null, true, "Credit Card", true },
                    { 2, null, true, "Debit Card", true },
                    { 3, null, true, "Cash", false },
                    { 4, null, true, "Check", false }
                });

            migrationBuilder.InsertData(
                table: "ReservationStatuses",
                columns: new[] { "ReservationStatusID", "Description", "StatusName" },
                values: new object[,]
                {
                    { 1, "Reservation is scheduled for future dates", "Upcoming" },
                    { 2, "Guest is currently checked in", "In Progress" },
                    { 3, "Reservation has been completed", "Completed" },
                    { 4, "Reservation was cancelled", "Cancelled" }
                });

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
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@rvpark.com", true, null, "Password123!", 2 },
                    { 2, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.doe@rvpark.com", true, null, "Password123!", 1 },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.smith@military.com", true, null, "Password123!", 0 }
                });

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "AdminID", "DateCreated", "FirstName", "LastName", "UserID" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Demo", "Admin", 1 });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerID", "DateCreated", "DoDStatus", "FirstName", "LastName", "MilitaryAffiliation", "PhoneNumber", "UserID" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "John", "Smith", 0, "555-123-4567", 3 });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeID", "AccessPermissions", "DateHired", "FirstName", "IsActive", "IsLockedOut", "LastName", "UserID" },
                values: new object[] { 1, 24, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jane", true, false, "Doe", 2 });

            migrationBuilder.InsertData(
                table: "SiteTypePricings",
                columns: new[] { "PricingID", "BasePrice", "Description", "EndDate", "SiteTypeID", "StartDate" },
                values: new object[,]
                {
                    { 1, 25.00m, "Current rate for standard trailers", null, 1, new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 30.00m, "Current rate for walk-in trailers", null, 2, new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 5.00m, "Current daily rate for dry storage", null, 3, new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 17.00m, "Current rate for tent site", null, 4, new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Sites",
                columns: new[] { "SiteID", "IsActive", "MaxLengthFeet", "Notes", "SiteNumber", "SiteTypeID" },
                values: new object[,]
                {
                    { 1, true, 40, null, "2", 1 },
                    { 2, true, 45, null, "17", 1 },
                    { 3, true, 65, null, "32", 1 },
                    { 4, true, 30, "Walk-in trailer with linens", "11B", 2 },
                    { 5, true, 30, "Walk-in trailer with linens", "12B", 2 },
                    { 6, true, 65, "Dry storage site", "A", 3 },
                    { 7, true, 65, "Dry storage site", "B", 3 },
                    { 8, true, 0, "Tent site near Dog Park", "TENT-1", 4 }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "ReservationID", "ActualCheckInTime", "ActualCheckOutTime", "BalanceDue", "BaseAmount", "CustomerID", "DateCreated", "EndDate", "LastUpdated", "Notes", "NumberOfGuests", "NumberOfPets", "ReservationStatusID", "ScheduledCheckInTime", "ScheduledCheckOutTime", "SiteID", "SpecialRequests", "StartDate", "TotalAmount", "TrailerLengthFeet" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 10, 13, 20, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 13, 11, 50, 0, 0, DateTimeKind.Unspecified), 0m, 75.00m, 1, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0, 0, 3, new DateTime(2026, 2, 10, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 13, 12, 0, 0, 0, DateTimeKind.Unspecified), 1, null, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 75.00m, 38 },
                    { 2, null, null, 0m, 100.00m, 1, new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cancelled - plans changed", 0, 0, 4, new DateTime(2026, 3, 5, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 9, 12, 0, 0, 0, DateTimeKind.Unspecified), 2, null, new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 110.00m, 42 },
                    { 3, null, null, 0m, 175.00m, 1, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0, 0, 1, new DateTime(2026, 4, 25, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 2, 12, 0, 0, 0, DateTimeKind.Unspecified), 3, null, new DateTime(2026, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 175.00m, 60 },
                    { 4, new DateTime(2026, 2, 20, 13, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 23, 11, 30, 0, 0, DateTimeKind.Unspecified), 0m, 90.00m, 1, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0, 0, 3, new DateTime(2026, 2, 20, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 23, 12, 0, 0, 0, DateTimeKind.Unspecified), 4, null, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 90.00m, null },
                    { 5, new DateTime(2026, 4, 18, 8, 10, 0, 0, DateTimeKind.Unspecified), null, 0m, 150.00m, 1, new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0, 0, 2, new DateTime(2026, 4, 18, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 23, 12, 0, 0, 0, DateTimeKind.Unspecified), 5, null, new DateTime(2026, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 165.00m, null },
                    { 6, null, null, 0m, 120.00m, 1, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0, 0, 1, new DateTime(2026, 5, 10, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 14, 12, 0, 0, 0, DateTimeKind.Unspecified), 4, null, new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 120.00m, null },
                    { 7, new DateTime(2026, 2, 1, 13, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 11, 12, 0, 0, 0, DateTimeKind.Unspecified), 0m, 50.00m, 1, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0, 0, 3, new DateTime(2026, 2, 1, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 11, 12, 0, 0, 0, DateTimeKind.Unspecified), 6, null, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 50.00m, 45 },
                    { 8, null, null, 0m, 30.00m, 1, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cancelled - vehicle sold", 0, 0, 4, new DateTime(2026, 3, 15, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 21, 12, 0, 0, 0, DateTimeKind.Unspecified), 7, null, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 40.00m, 50 },
                    { 9, null, null, 0m, 150.00m, 1, new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "30-day dry storage rental", 0, 0, 1, new DateTime(2026, 5, 1, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), 6, null, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 150.00m, 48 },
                    { 10, new DateTime(2026, 2, 28, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 2, 11, 45, 0, 0, DateTimeKind.Unspecified), 0m, 34.00m, 1, new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0, 0, 3, new DateTime(2026, 2, 28, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 2, 12, 0, 0, 0, DateTimeKind.Unspecified), 8, null, new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 34.00m, null },
                    { 11, new DateTime(2026, 4, 17, 13, 5, 0, 0, DateTimeKind.Unspecified), null, 0m, 51.00m, 1, new DateTime(2026, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0, 0, 2, new DateTime(2026, 4, 17, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 20, 12, 0, 0, 0, DateTimeKind.Unspecified), 8, null, new DateTime(2026, 4, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 76.00m, null },
                    { 12, null, null, 0m, 68.00m, 1, new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0, 0, 1, new DateTime(2026, 5, 5, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 9, 12, 0, 0, 0, DateTimeKind.Unspecified), 8, null, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 68.00m, null }
                });

            migrationBuilder.InsertData(
                table: "SitePhotos",
                columns: new[] { "PhotoID", "Caption", "DateUploaded", "DisplayOrder", "PhotoURL", "SiteID" },
                values: new object[,]
                {
                    { 1, "Site 2 - 40ft Trailer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0694.jpg?w=4032&ssl=1", 1 },
                    { 2, "Site 17 - 45ft Trailer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0954.jpg?w=4032&ssl=1", 2 },
                    { 3, "Site 32 - 65ft Trailer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1226.jpg?w=4032&ssl=1", 3 },
                    { 4, "Site 11B - Walk-in Trailer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1309.jpg?w=4032&ssl=1", 4 },
                    { 5, "Site 12B - Walk-in Trailer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0439.jpg?w=4032&ssl=1", 5 },
                    { 6, "Dry Storage A", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0603.jpg?w=4032&ssl=1", 6 },
                    { 7, "Dry Storage B", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0444.jpg?w=4032&ssl=1", 7 },
                    { 8, "Tent Site - Near Dog Park", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0515.jpg?w=4032&ssl=1", 8 },
                    { 9, "Site 17 - Hookup View", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0512.jpg?w=4032&ssl=1", 2 }
                });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "InvoiceID", "CustomerID", "DatePaid", "DueDate", "InvoiceDate", "IsPaid", "Notes", "ReservationID", "SubTotal", "TotalAmount", "TotalFees" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 1, 75.00m, 75.00m, 0m },
                    { 2, 1, new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 2, 100.00m, 110.00m, 10.00m },
                    { 3, 1, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 3, 175.00m, 175.00m, 0m },
                    { 4, 1, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 4, 90.00m, 90.00m, 0m },
                    { 5, 1, new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 5, 150.00m, 165.00m, 15.00m },
                    { 6, 1, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 6, 120.00m, 120.00m, 0m },
                    { 7, 1, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 7, 50.00m, 50.00m, 0m },
                    { 8, 1, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 8, 30.00m, 40.00m, 10.00m },
                    { 9, 1, new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 9, 150.00m, 150.00m, 0m },
                    { 10, 1, new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 10, 34.00m, 34.00m, 0m },
                    { 11, 1, new DateTime(2026, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 11, 51.00m, 76.00m, 25.00m },
                    { 12, 1, new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 12, 68.00m, 68.00m, 0m }
                });

            migrationBuilder.InsertData(
                table: "ReservationFees",
                columns: new[] { "ReservationFeeID", "Amount", "DateApplied", "FeeID", "Notes", "ReservationID" },
                values: new object[,]
                {
                    { 1, 10.00m, new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Cancellation fee applied", 2 },
                    { 2, 15.00m, new DateTime(2026, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Early check-in at 8:00 AM", 5 },
                    { 3, 10.00m, new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Cancellation fee applied", 8 },
                    { 4, 25.00m, new DateTime(2026, 4, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Late checkout requested", 11 }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentID", "Amount", "InvoiceID", "IsRefund", "Notes", "PaymentDate", "PaymentMethodID", "PaymentStatus", "ProcessedByEmployeeID", "StripeTransactionID" },
                values: new object[,]
                {
                    { 1, 75.00m, 1, false, null, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Completed", null, "txn_demo_001" },
                    { 2, 110.00m, 2, false, null, new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Completed", null, "txn_demo_002" },
                    { 3, -100.00m, 2, true, "Refund: $100.00 (kept $10.00 cancellation fee)", new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Refunded", null, "txn_demo_002_refund" },
                    { 4, 175.00m, 3, false, null, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Completed", null, "txn_demo_003" },
                    { 5, 90.00m, 4, false, null, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Completed", null, "txn_demo_004" },
                    { 6, 165.00m, 5, false, null, new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Completed", null, "txn_demo_005" },
                    { 7, 120.00m, 6, false, null, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Completed", null, "txn_demo_006" },
                    { 8, 50.00m, 7, false, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Completed", 1, null },
                    { 9, 40.00m, 8, false, null, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Completed", null, "txn_demo_008" },
                    { 10, -30.00m, 8, true, "Refund: $30.00 (kept $10.00 cancellation fee)", new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Refunded", null, "txn_demo_008_refund" },
                    { 11, 150.00m, 9, false, null, new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Completed", 1, null },
                    { 12, 34.00m, 10, false, null, new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Completed", null, "txn_demo_010" },
                    { 13, 76.00m, 11, false, null, new DateTime(2026, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Completed", null, "txn_demo_011" },
                    { 14, 68.00m, 12, false, null, new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Completed", null, "txn_demo_012" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UserID",
                table: "Admins",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserID",
                table: "Customers",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserID",
                table: "Employees",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerID",
                table: "Invoices",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ReservationID",
                table: "Invoices",
                column: "ReservationID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InvoiceID",
                table: "Payments",
                column: "InvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentMethodID",
                table: "Payments",
                column: "PaymentMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ProcessedByEmployeeID",
                table: "Payments",
                column: "ProcessedByEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationFees_FeeID",
                table: "ReservationFees",
                column: "FeeID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationFees_ReservationID",
                table: "ReservationFees",
                column: "ReservationID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CustomerID",
                table: "Reservations",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReservationStatusID",
                table: "Reservations",
                column: "ReservationStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_SiteID",
                table: "Reservations",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_SitePhotos_SiteID",
                table: "SitePhotos",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_SiteNumber",
                table: "Sites",
                column: "SiteNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sites_SiteTypeID",
                table: "Sites",
                column: "SiteTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_SiteTypePricings_SiteTypeID",
                table: "SiteTypePricings",
                column: "SiteTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ReservationFees");

            migrationBuilder.DropTable(
                name: "SitePhotos");

            migrationBuilder.DropTable(
                name: "SiteTypePricings");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "Fees");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "ReservationStatuses");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "SiteTypes");
        }
    }
}
