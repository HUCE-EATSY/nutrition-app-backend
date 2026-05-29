using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nutrition_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoUrlToWeightLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "weight_logs",
                type: "varchar(2048)",
                maxLength: 2048,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "weight_logs");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5822), new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5827) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5831), new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5832) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5835), new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5836) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5839), new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5840) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5843), new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5844) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5847), new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5848) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5851), new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5851) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5855), new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5855) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5859), new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5860) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5863), new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5864) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5867), new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5868) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5871), new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5872) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5875), new DateTime(2026, 5, 29, 14, 5, 21, 583, DateTimeKind.Utc).AddTicks(5875) });
        }
    }
}
