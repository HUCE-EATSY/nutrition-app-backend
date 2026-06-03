using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nutrition_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuAndDailyPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "daily_plans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LogDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FoodItemId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    MealTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    QuantityG = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    IsSynced = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_plans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_daily_plans_food_items_FoodItemId",
                        column: x => x.FoodItemId,
                        principalTable: "food_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_daily_plans_meal_types_MealTypeId",
                        column: x => x.MealTypeId,
                        principalTable: "meal_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_daily_plans_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "menus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CoverImageUrl = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    TotalCalories = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalProtein = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCarbs = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalFat = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
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
                    MealTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    QuantityG = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false)
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
                        name: "FK_menu_foods_meal_types_MealTypeId",
                        column: x => x.MealTypeId,
                        principalTable: "meal_types",
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
                values: new object[] { new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4654), new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4657) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4661), new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4661) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4665), new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4666) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4668), new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4669) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4671), new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4671) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4673), new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4673) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4675), new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4676) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4678), new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4678) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4680), new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4680) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4682), new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4683) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4684), new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4685) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4687), new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4687) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4689), new DateTime(2026, 6, 2, 13, 6, 23, 327, DateTimeKind.Utc).AddTicks(4689) });

            migrationBuilder.CreateIndex(
                name: "idx_daily_plan_user_date",
                table: "daily_plans",
                columns: new[] { "UserId", "LogDate" });

            migrationBuilder.CreateIndex(
                name: "IX_daily_plans_FoodItemId",
                table: "daily_plans",
                column: "FoodItemId");

            migrationBuilder.CreateIndex(
                name: "IX_daily_plans_MealTypeId",
                table: "daily_plans",
                column: "MealTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_menu_foods_FoodItemId",
                table: "menu_foods",
                column: "FoodItemId");

            migrationBuilder.CreateIndex(
                name: "IX_menu_foods_MealTypeId",
                table: "menu_foods",
                column: "MealTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_menu_foods_MenuId",
                table: "menu_foods",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_menus_UserId",
                table: "menus",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "daily_plans");

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
