using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nutrition_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddStepLogsAndHealthConnections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "step_logs",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LogDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Steps = table.Column<int>(type: "int", nullable: false),
                    StepGoal = table.Column<int>(type: "int", nullable: false),
                    Provider = table.Column<byte>(type: "tinyint unsigned", nullable: true),
                    CaloriesBurnedKcal = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_step_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_step_logs_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_health_connections",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Provider = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Status = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ConnectedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_health_connections", x => new { x.UserId, x.Provider });
                    table.ForeignKey(
                        name: "FK_user_health_connections_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "idx_steps_user_date",
                table: "step_logs",
                columns: new[] { "UserId", "LogDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "step_logs");

            migrationBuilder.DropTable(
                name: "user_health_connections");

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
