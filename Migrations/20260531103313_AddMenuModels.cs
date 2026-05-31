using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nutrition_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "menus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_menus_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "menu_foods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    MenuId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FoodItemId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    QuantityG = table.Column<decimal>(type: "decimal(8,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_foods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_menu_foods_food_items_FoodItemId",
                        column: x => x.FoodItemId,
                        principalTable: "food_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_menu_foods_menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8410), new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8414) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8418), new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8418) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8421), new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8422) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8424), new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8424) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8426), new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8427) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8429), new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8429) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8431), new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8431) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8433), new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8434) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8436), new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8436) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8438), new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8438) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8440), new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8440) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8442), new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8443) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8445), new DateTime(2026, 5, 31, 10, 33, 12, 283, DateTimeKind.Utc).AddTicks(8445) });

            migrationBuilder.CreateIndex(
                name: "IX_menu_foods_FoodItemId",
                table: "menu_foods",
                column: "FoodItemId");

            migrationBuilder.CreateIndex(
                name: "IX_menu_foods_MenuId_FoodItemId",
                table: "menu_foods",
                columns: new[] { "MenuId", "FoodItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_menu_user",
                table: "menus",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "menu_foods");

            migrationBuilder.DropTable(
                name: "menus");

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
    }
}
