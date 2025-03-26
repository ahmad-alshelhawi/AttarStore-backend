using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttarStore.Services.Migrations
{
    /// <inheritdoc />
    public partial class init15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 26, 11, 32, 56, 674, DateTimeKind.Unspecified).AddTicks(4793), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$kKfhOBrvKFfztmHf8ZyEHOXTPGrqwr4O0k5kYa1ELerH3yJqImezG", new DateTime(2025, 3, 26, 7, 33, 56, 674, DateTimeKind.Utc).AddTicks(4533) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_at", "Password", "RefreshTokenExpiryTime" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 3, 24, 9, 45, 41, 922, DateTimeKind.Unspecified).AddTicks(6193), new TimeSpan(0, 4, 0, 0, 0)), "$2a$11$q/xKby4Oqjl2UhAwmi6nouIKW0R2pDrERd9BWzOyXhnz4fypI726i", new DateTime(2025, 3, 24, 5, 46, 41, 922, DateTimeKind.Utc).AddTicks(5958) });
        }
    }
}
