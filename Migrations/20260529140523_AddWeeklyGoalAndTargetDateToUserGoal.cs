using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nutrition_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddWeeklyGoalAndTargetDateToUserGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TargetDate",
                table: "user_goals",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "WeeklyGoalKg",
                table: "user_goals",
                type: "decimal(3,2)",
                precision: 3,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetDate",
                table: "user_goals");

            migrationBuilder.DropColumn(
                name: "WeeklyGoalKg",
                table: "user_goals");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5559), new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5563) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5571), new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5571) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5575), new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5575) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5577), new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5578) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5580), new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5580) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5582), new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5582) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5584), new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5585) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5587), new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5587) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5589), new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5589) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5591), new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5592) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5594), new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5594) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5596), new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5597) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5599), new DateTime(2026, 5, 28, 4, 30, 3, 542, DateTimeKind.Utc).AddTicks(5599) });
        }
    }
}
