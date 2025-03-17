using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Services.Migrations
{
    /// <inheritdoc />
    public partial class init8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 14, 15, 39, 50, 778, DateTimeKind.Unspecified).AddTicks(383), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$YRtujGUR.UnDlw7lLBSz.Oc0BVutfqJghz4eoRLD00/SbhSchXNwC", new DateTime(2025, 3, 14, 11, 40, 50, 777, DateTimeKind.Utc).AddTicks(9948) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 14, 11, 45, 34, 329, DateTimeKind.Unspecified).AddTicks(3125), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$7q5OdjW/t9ix4JVbj4O/guqOlDwaxCUj2i6v1m52WmBDVnD8G2Lse", new DateTime(2025, 3, 14, 7, 46, 34, 329, DateTimeKind.Utc).AddTicks(2801) });
        }
    }
}
