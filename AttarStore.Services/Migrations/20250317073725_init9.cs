using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Services.Migrations
{
    /// <inheritdoc />
    public partial class init9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpiry",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetToken",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpiry",
                table: "Clients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetToken",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpiry",
                table: "Admins",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime", "ResetToken", "ResetTokenExpiry", "Role" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 17, 11, 37, 24, 687, DateTimeKind.Unspecified).AddTicks(5031), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$3CKLeWnswgmBLyJsC9yr9eru0YWGNC0mw7z7J39SKVLw5r1ErhLoy", new DateTime(2025, 3, 17, 7, 38, 24, 687, DateTimeKind.Utc).AddTicks(4392), null, null, "SuperAdmin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpiry",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetToken",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpiry",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ResetToken",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpiry",
                table: "Admins");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime", "Role" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 14, 15, 39, 50, 778, DateTimeKind.Unspecified).AddTicks(383), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$YRtujGUR.UnDlw7lLBSz.Oc0BVutfqJghz4eoRLD00/SbhSchXNwC", new DateTime(2025, 3, 14, 11, 40, 50, 777, DateTimeKind.Utc).AddTicks(9948), "Master" });
        }
    }
}
