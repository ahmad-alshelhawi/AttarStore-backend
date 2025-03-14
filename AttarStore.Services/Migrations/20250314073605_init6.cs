using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Services.Migrations
{
    /// <inheritdoc />
    public partial class init6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 14, 11, 36, 4, 909, DateTimeKind.Unspecified).AddTicks(2696), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$fiQ8nvWeuVKxN426oOv2I.S1euN9u8BZviE4NZ1L/Ihhy0Pvo7GBa", new DateTime(2025, 3, 14, 7, 37, 4, 909, DateTimeKind.Utc).AddTicks(2406) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 14, 10, 4, 24, 861, DateTimeKind.Unspecified).AddTicks(5542), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$sevJfjjKhFmTk5IRaIuK6.qCYSWaEb3aPDoOv69QRaw4fJRExFdIG", new DateTime(2025, 3, 14, 6, 5, 24, 861, DateTimeKind.Utc).AddTicks(4921) });
        }
    }
}
