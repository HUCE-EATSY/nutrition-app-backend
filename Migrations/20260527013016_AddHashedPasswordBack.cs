using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nutrition_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddHashedPasswordBack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "hashed_password",
                table: "user_auth_providers",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1540), new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1543) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1548), new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1549) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1552), new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1553) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1556), new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1557) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1560), new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1561) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1614), new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1614) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1618), new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1619) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1622), new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1623) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1626), new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1627) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1630), new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1631) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1634), new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1635) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1638), new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1639) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1643), new DateTime(2026, 5, 27, 1, 30, 14, 582, DateTimeKind.Utc).AddTicks(1643) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hashed_password",
                table: "user_auth_providers");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3090), new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3094) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3100), new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3101) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3105), new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3109) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3125), new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3126) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3130), new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3130) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3134), new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3135) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3138), new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3139) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3142), new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3143) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3146), new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3147) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3150), new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3151) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3155), new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3155) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3159), new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3159) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3163), new DateTime(2026, 5, 26, 2, 56, 29, 963, DateTimeKind.Utc).AddTicks(3164) });
        }
    }
}
