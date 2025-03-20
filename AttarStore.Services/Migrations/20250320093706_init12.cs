using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Services.Migrations
{
    /// <inheritdoc />
    public partial class init12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 20, 13, 37, 5, 782, DateTimeKind.Unspecified).AddTicks(4974), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$1C/KF5CIUt/1NCpRc0wJKuV2.6LSduix3XHUdejUEJH6BSP6MrDeC", new DateTime(2025, 3, 20, 9, 38, 5, 782, DateTimeKind.Utc).AddTicks(4439) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Categories");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 20, 11, 28, 44, 178, DateTimeKind.Unspecified).AddTicks(4863), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$HiRsMZFienWzAZDvbbBxXeRjG8Gq1DVhnz4LAjyFgvUoYQqUATXMq", new DateTime(2025, 3, 20, 7, 29, 44, 178, DateTimeKind.Utc).AddTicks(4609) });
        }
    }
}
