using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nutrition_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDeviceToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_device_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    DeviceToken = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_device_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_device_tokens_users_UserId",
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
                values: new object[] { new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5926), new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5929) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5934), new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5934) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5936), new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5937) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5939), new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5939) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5941), new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5942) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5943), new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5944) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5946), new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5947) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5949), new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5949) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5951), new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5952) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5954), new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5954) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5956), new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5957) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5959), new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5959) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5961), new DateTime(2026, 5, 30, 14, 42, 50, 211, DateTimeKind.Utc).AddTicks(5961) });

            migrationBuilder.CreateIndex(
                name: "idx_user_device_token_unique",
                table: "user_device_tokens",
                column: "DeviceToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_device_tokens_UserId",
                table: "user_device_tokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_device_tokens");

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
