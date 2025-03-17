using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Services.Migrations
{
    /// <inheritdoc />
    public partial class init11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ResetToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ResetToken",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime", "Role" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 17, 12, 10, 8, 120, DateTimeKind.Unspecified).AddTicks(20), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$m7iiPtGakB/q8J.VvC7VuOH9gSChRJHlgoB/NiKRp9YnusI6QvzK6", new DateTime(2025, 3, 17, 8, 11, 8, 119, DateTimeKind.Utc).AddTicks(9547), "Master" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ResetToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResetToken",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime", "Role" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 17, 11, 37, 24, 687, DateTimeKind.Unspecified).AddTicks(5031), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$3CKLeWnswgmBLyJsC9yr9eru0YWGNC0mw7z7J39SKVLw5r1ErhLoy", new DateTime(2025, 3, 17, 7, 38, 24, 687, DateTimeKind.Utc).AddTicks(4392), "SuperAdmin" });
        }
    }
}
