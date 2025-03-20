using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Services.Migrations
{
    /// <inheritdoc />
    public partial class init13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 20, 11, 28, 44, 178, DateTimeKind.Unspecified).AddTicks(4863), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$HiRsMZFienWzAZDvbbBxXeRjG8Gq1DVhnz4LAjyFgvUoYQqUATXMq", new DateTime(2025, 3, 20, 7, 29, 44, 178, DateTimeKind.Utc).AddTicks(4609) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Inventories");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Inventories",
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
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 17, 12, 10, 8, 120, DateTimeKind.Unspecified).AddTicks(20), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$m7iiPtGakB/q8J.VvC7VuOH9gSChRJHlgoB/NiKRp9YnusI6QvzK6", new DateTime(2025, 3, 17, 8, 11, 8, 119, DateTimeKind.Utc).AddTicks(9547) });
        }
    }
}
