using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Services.Migrations
{
    /// <inheritdoc />
    public partial class init18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 4, 2, 22, 31, 34, 904, DateTimeKind.Unspecified).AddTicks(7626), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$jAqjT2aEpaXrVRPME/4nzuzVW3kROrADmQT6R0qbwkeSPHGSnyNS.", new DateTime(2025, 4, 2, 18, 32, 34, 904, DateTimeKind.Utc).AddTicks(7228) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 26, 13, 38, 38, 21, DateTimeKind.Unspecified).AddTicks(3695), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$41DxJZMGFLXeQrMK4Arq2OzlPitiwqs7KAqJ84lXQQJp57t.l0a8u", new DateTime(2025, 3, 26, 9, 39, 38, 21, DateTimeKind.Utc).AddTicks(3085) });
        }
    }
}
