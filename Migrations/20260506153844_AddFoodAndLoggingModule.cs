using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace nutrition_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddFoodAndLoggingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "food_categories",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    NameVi = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NameEn = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_food_categories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "meal_types",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    NameVi = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meal_types", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "weight_logs",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    WeightKg = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    LogDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_weight_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_weight_logs_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "food_item_components",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentFoodId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    ChildFoodId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    QuantityG = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_food_item_components", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "food_item_images",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FoodItemId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    StoragePath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StorageProvider = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_food_item_images", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "food_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    NameVi = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NameEn = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    CategoryId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Source = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Status = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ServingSizeG = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    ServingUnitVi = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ThumbnailUrl = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActiveImageId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    Barcode = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_food_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_food_items_food_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "food_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_food_items_food_item_images_ActiveImageId",
                        column: x => x.ActiveImageId,
                        principalTable: "food_item_images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_food_items_food_items_ParentId",
                        column: x => x.ParentId,
                        principalTable: "food_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_food_items_users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "food_logs",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FoodItemId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    MealTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    LogDate = table.Column<DateOnly>(type: "date", nullable: false),
                    QuantityG = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    CaloriesKcal = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    ProteinG = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    CarbsG = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    FatG = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    InputMethod = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_food_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_food_logs_food_items_FoodItemId",
                        column: x => x.FoodItemId,
                        principalTable: "food_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_food_logs_meal_types_MealTypeId",
                        column: x => x.MealTypeId,
                        principalTable: "meal_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_food_logs_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "food_nutrition",
                columns: table => new
                {
                    FoodItemId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    CaloriesKcal = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    ProteinG = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    CarbsG = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    FatG = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    FiberG = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: true),
                    SugarG = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: true),
                    SodiumMg = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_food_nutrition", x => x.FoodItemId);
                    table.ForeignKey(
                        name: "FK_food_nutrition_food_items_FoodItemId",
                        column: x => x.FoodItemId,
                        principalTable: "food_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "food_categories",
                columns: new[] { "Id", "NameEn", "NameVi" },
                values: new object[,]
                {
                    { (byte)1, "Rice dishes", "Cơm & Xôi" },
                    { (byte)2, "Noodle soups", "Phở & Bún" },
                    { (byte)3, "Bread & Pastries", "Bánh mì & Bánh" },
                    { (byte)4, "Beverages", "Đồ uống" },
                    { (byte)5, "Packaged food", "Thực phẩm đóng gói" },
                    { (byte)6, "Vegetables & Fruits", "Rau củ quả" },
                    { (byte)7, "Meat & Seafood", "Thịt & Hải sản" },
                    { (byte)8, "F&B Chains", "Chuỗi F&B" },
                    { (byte)9, "International", "Quốc tế" },
                    { (byte)10, "Other", "Khác" }
                });

            migrationBuilder.InsertData(
                table: "meal_types",
                columns: new[] { "Id", "NameVi" },
                values: new object[,]
                {
                    { (byte)1, "Bữa sáng" },
                    { (byte)2, "Bữa trưa" },
                    { (byte)3, "Bữa tối" },
                    { (byte)4, "Bữa phụ" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_food_item_components_ChildFoodId",
                table: "food_item_components",
                column: "ChildFoodId");

            migrationBuilder.CreateIndex(
                name: "IX_food_item_components_ParentFoodId_ChildFoodId",
                table: "food_item_components",
                columns: new[] { "ParentFoodId", "ChildFoodId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_img_food",
                table: "food_item_images",
                column: "FoodItemId");

            migrationBuilder.CreateIndex(
                name: "idx_food_active_image",
                table: "food_items",
                column: "ActiveImageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_food_ft",
                table: "food_items",
                columns: new[] { "NameVi", "NameEn" })
                .Annotation("MySql:FullTextIndex", true);

            migrationBuilder.CreateIndex(
                name: "idx_food_status",
                table: "food_items",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_food_items_CategoryId",
                table: "food_items",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_food_items_CreatedBy",
                table: "food_items",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_food_items_ParentId",
                table: "food_items",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "idx_logs_user_date",
                table: "food_logs",
                columns: new[] { "UserId", "LogDate" });

            migrationBuilder.CreateIndex(
                name: "IX_food_logs_FoodItemId",
                table: "food_logs",
                column: "FoodItemId");

            migrationBuilder.CreateIndex(
                name: "IX_food_logs_MealTypeId",
                table: "food_logs",
                column: "MealTypeId");

            migrationBuilder.CreateIndex(
                name: "idx_weight_user_date",
                table: "weight_logs",
                columns: new[] { "UserId", "LogDate" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_food_item_components_food_items_ChildFoodId",
                table: "food_item_components",
                column: "ChildFoodId",
                principalTable: "food_items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_food_item_components_food_items_ParentFoodId",
                table: "food_item_components",
                column: "ParentFoodId",
                principalTable: "food_items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_food_item_images_food_items_FoodItemId",
                table: "food_item_images",
                column: "FoodItemId",
                principalTable: "food_items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_food_item_images_food_items_FoodItemId",
                table: "food_item_images");

            migrationBuilder.DropTable(
                name: "food_item_components");

            migrationBuilder.DropTable(
                name: "food_logs");

            migrationBuilder.DropTable(
                name: "food_nutrition");

            migrationBuilder.DropTable(
                name: "weight_logs");

            migrationBuilder.DropTable(
                name: "meal_types");

            migrationBuilder.DropTable(
                name: "food_items");

            migrationBuilder.DropTable(
                name: "food_categories");

            migrationBuilder.DropTable(
                name: "food_item_images");
        }
    }
}
