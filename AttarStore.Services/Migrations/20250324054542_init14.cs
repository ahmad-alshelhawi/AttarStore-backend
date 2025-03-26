using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Services.Migrations
{
    /// <inheritdoc />
    public partial class init14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 24, 9, 45, 41, 922, DateTimeKind.Unspecified).AddTicks(6193), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$q/xKby4Oqjl2UhAwmi6nouIKW0R2pDrERd9BWzOyXhnz4fypI726i", new DateTime(2025, 3, 24, 5, 46, 41, 922, DateTimeKind.Utc).AddTicks(5958) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Products");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 20, 13, 37, 5, 782, DateTimeKind.Unspecified).AddTicks(4974), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$1C/KF5CIUt/1NCpRc0wJKuV2.6LSduix3XHUdejUEJH6BSP6MrDeC", new DateTime(2025, 3, 20, 9, 38, 5, 782, DateTimeKind.Utc).AddTicks(4439) });
        }
    }
}
