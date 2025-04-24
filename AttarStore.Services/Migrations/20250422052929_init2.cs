using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Services.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 4, 22, 9, 29, 29, 325, DateTimeKind.Unspecified).AddTicks(5986), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$R3pA53HbqIzFwVSbUHcAQ.0I.J97vp4oh5j3//mGuyFml90fX8uDO", new DateTime(2025, 4, 22, 5, 30, 29, 325, DateTimeKind.Utc).AddTicks(5874) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 4, 22, 9, 27, 10, 413, DateTimeKind.Unspecified).AddTicks(2653), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$f2fpCdeTZWDtE.3olYNq1eFWi49LXhdjXRQDWuEvoNhevejmjwBsu", new DateTime(2025, 4, 22, 5, 28, 10, 413, DateTimeKind.Utc).AddTicks(2523) });
        }
    }
}
