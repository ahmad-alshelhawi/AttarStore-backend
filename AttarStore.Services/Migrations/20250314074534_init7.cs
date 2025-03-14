using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Services.Migrations
{
    /// <inheritdoc />
    public partial class init7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 14, 11, 45, 34, 329, DateTimeKind.Unspecified).AddTicks(3125), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$7q5OdjW/t9ix4JVbj4O/guqOlDwaxCUj2i6v1m52WmBDVnD8G2Lse", new DateTime(2025, 3, 14, 7, 46, 34, 329, DateTimeKind.Utc).AddTicks(2801) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 14, 11, 36, 4, 909, DateTimeKind.Unspecified).AddTicks(2696), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$fiQ8nvWeuVKxN426oOv2I.S1euN9u8BZviE4NZ1L/Ihhy0Pvo7GBa", new DateTime(2025, 3, 14, 7, 37, 4, 909, DateTimeKind.Utc).AddTicks(2406) });
        }
    }
}
