using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Services.Migrations
{
    /// <inheritdoc />
    public partial class init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Carts_CartId",
                table: "Clients");

            migrationBuilder.AlterColumn<int>(
                name: "CartId",
                table: "Clients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 4, 15, 15, 2, 11, 236, DateTimeKind.Unspecified).AddTicks(8545), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$bFAs.pk3FPRjhdHPe5GZqeEIJye.3dcQKYJsWe8TqVlxOuC8.wk6O", new DateTime(2025, 4, 15, 11, 3, 11, 236, DateTimeKind.Utc).AddTicks(8209) });

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Carts_CartId",
                table: "Clients",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Carts_CartId",
                table: "Clients");

            migrationBuilder.AlterColumn<int>(
                name: "CartId",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 4, 15, 14, 59, 8, 470, DateTimeKind.Unspecified).AddTicks(3439), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$j7bdhROnxKabdHmC9w8RaOsUDZK1hdlVhXmkSWs7By6u6RPObgGGe", new DateTime(2025, 4, 15, 11, 0, 8, 470, DateTimeKind.Utc).AddTicks(3150) });

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Carts_CartId",
                table: "Clients",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
