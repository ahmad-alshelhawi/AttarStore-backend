using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Services.Migrations
{
    /// <inheritdoc />
    public partial class init5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 14, 10, 4, 24, 861, DateTimeKind.Unspecified).AddTicks(5542), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$sevJfjjKhFmTk5IRaIuK6.qCYSWaEb3aPDoOv69QRaw4fJRExFdIG", new DateTime(2025, 3, 14, 6, 5, 24, 861, DateTimeKind.Utc).AddTicks(4921) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 13, 15, 2, 25, 70, DateTimeKind.Unspecified).AddTicks(6796), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$t5AyRCQ4mX.akvblMEjpX.jVFGagkIx4fkpD/Ysq/xxGEKdTakPJ.", new DateTime(2025, 3, 13, 11, 3, 25, 70, DateTimeKind.Utc).AddTicks(6459) });
        }
    }
}
