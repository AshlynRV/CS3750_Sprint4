using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SiteReservationSystem.Web.Migrations
{
    /// <inheritdoc />
    public partial class SeedImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SitePhotos",
                columns: new[] { "PhotoID", "Caption", "DateUploaded", "DisplayOrder", "PhotoURL", "SiteID" },
                values: new object[,]
                {
                    { 1, "Site 2 - Overview", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0694.jpg?w=4032&ssl=1", 1 },
                    { 2, "Site 2 - Hookup View", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/09/IMG_1864.jpg?w=4032&ssl=1", 1 },
                    { 3, "Site 3", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/09/IMG_1843.jpg?w=4032&ssl=1", 2 },
                    { 4, "Site 4", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/09/IMG_1844.jpg?w=4032&ssl=1", 3 },
                    { 5, "Site 5", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/10/IMG_2113.jpg?w=4032&ssl=1", 4 },
                    { 6, "Site 6", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0401.jpg?w=4032&ssl=1", 5 },
                    { 7, "Site 7", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0710.jpg?w=4032&ssl=1", 6 },
                    { 8, "Site 8", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/09/IMG_1855-1.jpg?w=4032&ssl=1", 7 },
                    { 9, "Site 9", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1345.jpg?w=4032&ssl=1", 8 },
                    { 10, "Site 10", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1054.jpg?w=4032&ssl=1", 9 },
                    { 11, "Site 11", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1039.jpg?w=4032&ssl=1", 10 },
                    { 12, "Site 12", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/09/IMG_1860.jpg?w=4032&ssl=1", 11 },
                    { 13, "Site 13", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1141.jpg?w=4032&ssl=1", 12 },
                    { 14, "Site 14", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0419.jpg?w=4032&ssl=1", 13 },
                    { 15, "Site 17", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0954.jpg?w=4032&ssl=1", 14 },
                    { 16, "Site 18", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0512.jpg?w=4032&ssl=1", 15 },
                    { 17, "Site 20", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0596.jpg?w=4032&ssl=1", 16 },
                    { 18, "Site 22", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0659.jpg?w=4032&ssl=1", 17 },
                    { 19, "Site 23", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0408.jpg?w=4032&ssl=1", 18 },
                    { 20, "Site 24", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1321.jpg?w=4032&ssl=1", 19 },
                    { 21, "Site 25", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0409.jpg?w=4032&ssl=1", 20 },
                    { 22, "Site 26", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0403.jpg?w=4032&ssl=1", 21 },
                    { 23, "Site 27", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1223-1.jpg?w=4032&ssl=1", 22 },
                    { 24, "Site 28", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0407.jpg?w=4032&ssl=1", 23 },
                    { 25, "Site 29", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1164.jpg?w=4032&ssl=1", 24 },
                    { 26, "Site 30", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/04/IMG_20190422_100028.jpg?w=4032&ssl=1", 25 },
                    { 27, "Site 31", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1065.jpg?w=4032&ssl=1", 26 },
                    { 28, "Site 32", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1226.jpg?w=4032&ssl=1", 27 },
                    { 29, "Site 33", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0982.jpg?w=4032&ssl=1", 28 },
                    { 30, "Site 34", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0988.jpg?w=4032&ssl=1", 29 },
                    { 31, "Site 35", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1173.jpg?w=4032&ssl=1", 30 },
                    { 32, "Site 36", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1269.jpg?w=4032&ssl=1", 31 },
                    { 33, "Site 37", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0640.jpg?w=4032&ssl=1", 32 },
                    { 34, "Site 38", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1199.jpg?w=4032&ssl=1", 33 },
                    { 35, "Site 39", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1377.jpg?w=4032&ssl=1", 34 },
                    { 36, "Site 40", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0617.jpg?w=4032&ssl=1", 35 },
                    { 37, "Site 41", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1004.jpg?w=4032&ssl=1", 36 },
                    { 38, "Site 42", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1315.jpg?w=4032&ssl=1", 37 },
                    { 39, "Site 43", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0390.jpg?w=4032&ssl=1", 38 },
                    { 40, "Site 44", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0612.jpg?w=4032&ssl=1", 39 },
                    { 41, "Site 45", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_20190430_100259.jpg?w=4032&ssl=1", 40 },
                    { 42, "Site 1 (55ft exception)", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1304.jpg?w=4032&ssl=1", 41 },
                    { 43, "Site 19 (55ft exception)", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_20190430_100506.jpg?w=4032&ssl=1", 42 },
                    { 44, "Site 21 (55ft exception)", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1019.jpg?w=4032&ssl=1", 43 },
                    { 45, "Site 11B - Walk-in Trailer", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1309.jpg?w=4032&ssl=1", 44 },
                    { 46, "Site 12B - Walk-in Trailer", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0439.jpg?w=4032&ssl=1", 45 },
                    { 47, "Dry Storage A", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0603.jpg?w=4032&ssl=1", 46 },
                    { 48, "Dry Storage B", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0444.jpg?w=4032&ssl=1", 47 },
                    { 49, "Dry Storage C", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0976.jpg?w=4032&ssl=1", 48 },
                    { 50, "Dry Storage D", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0413.jpg?w=4032&ssl=1", 49 },
                    { 51, "Tent Site - Near Dog Park", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0515.jpg?w=4032&ssl=1", 50 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "SitePhotos",
                keyColumn: "PhotoID",
                keyValue: 51);
        }
    }
}
