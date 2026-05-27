using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nutrition_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBarcodeToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "food_items",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5164), new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5167) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5176), new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5177) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5181), new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5182) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5186), new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5187) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5192), new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5192) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5196), new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5197) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5201), new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5201) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5209), new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5210) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5214), new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5214) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5219), new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5220) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5224), new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5225) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5228), new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5229) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5233), new DateTime(2026, 5, 27, 4, 55, 53, 760, DateTimeKind.Utc).AddTicks(5234) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "Barcode",
                table: "food_items",
                type: "bigint unsigned",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2552), new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2556) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2558), new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2559) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2561), new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2561) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2564), new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2564) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2566), new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2566) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2569), new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2569) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2571), new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2572) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2573), new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2573) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2576), new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2576) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2657), new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2658) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2660), new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2661) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2662), new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2663) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2665), new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2665) });
        }
    }
}
