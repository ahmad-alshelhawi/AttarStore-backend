using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Services.Migrations
{
    /// <inheritdoc />
    public partial class init17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 26, 13, 38, 38, 21, DateTimeKind.Unspecified).AddTicks(3695), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$41DxJZMGFLXeQrMK4Arq2OzlPitiwqs7KAqJ84lXQQJp57t.l0a8u", new DateTime(2025, 3, 26, 9, 39, 38, 21, DateTimeKind.Utc).AddTicks(3085) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 26, 11, 32, 56, 674, DateTimeKind.Unspecified).AddTicks(4793), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$kKfhOBrvKFfztmHf8ZyEHOXTPGrqwr4O0k5kYa1ELerH3yJqImezG", new DateTime(2025, 3, 26, 7, 33, 56, 674, DateTimeKind.Utc).AddTicks(4533) });
        }
    }
}
