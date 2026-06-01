using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nutrition_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFoodLogDateToDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LogDate",
                table: "food_logs",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7207), new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7210) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7216), new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7217) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7220), new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7221) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7224), new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7225) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7230), new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7230) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7235), new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7235) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7241), new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7242) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7245), new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7246) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7249), new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7250) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7253), new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7254) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7257), new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7258) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7261), new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7262) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7265), new DateTime(2026, 5, 29, 14, 26, 49, 2, DateTimeKind.Utc).AddTicks(7266) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "LogDate",
                table: "food_logs",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6875), new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6878) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6885), new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6886) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6890), new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6891) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6896), new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6896) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6901), new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6902) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6905), new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6906) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6909), new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6910) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6914), new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6914) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6918), new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6919) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6922), new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6923) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6926), new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6927) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6930), new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6931) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6934), new DateTime(2026, 5, 29, 14, 12, 6, 341, DateTimeKind.Utc).AddTicks(6935) });
        }
    }
}
