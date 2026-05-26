using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace nutrition_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseAndNotificationModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exercise_categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NameVi = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NameEn = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IconUrl = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_categories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notification_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NameVi = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NameEn = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_types", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "exercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    NameVi = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NameEn = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MetValue = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Unit = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IconUrl = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_exercises_exercise_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "exercise_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    NotificationTypeId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Data = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notifications_notification_types_NotificationTypeId",
                        column: x => x.NotificationTypeId,
                        principalTable: "notification_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notifications_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_notification_settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    NotificationTypeId = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReminderTime = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DaysOfWeek = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_notification_settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_notification_settings_notification_types_NotificationTy~",
                        column: x => x.NotificationTypeId,
                        principalTable: "notification_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_notification_settings_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "exercise_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    ExerciseId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LogDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Intensity = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    CaloriesBurned = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_exercise_logs_exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_exercise_logs_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "exercise_categories",
                columns: new[] { "Id", "DisplayOrder", "IconUrl", "NameEn", "NameVi" },
                values: new object[,]
                {
                    { 1, 1, null, "Cardio", "Cardio" },
                    { 2, 2, null, "Strength", "Sức mạnh" },
                    { 3, 3, null, "Yoga & Pilates", "Yoga & Pilates" },
                    { 4, 4, null, "Sports", "Thể thao" },
                    { 5, 5, null, "Other", "Khác" }
                });

            migrationBuilder.InsertData(
                table: "notification_types",
                columns: new[] { "Id", "Code", "Description", "NameEn", "NameVi" },
                values: new object[,]
                {
                    { 1, "MEAL_REMINDER", "Nhắc nhở ghi nhật ký bữa ăn", "Meal Reminder", "Nhắc nhở bữa ăn" },
                    { 2, "EXERCISE_REMINDER", "Nhắc nhở ghi nhật ký tập luyện", "Exercise Reminder", "Nhắc nhở tập luyện" },
                    { 3, "WEIGHT_LOG_REMINDER", "Nhắc nhở ghi lại cân nặng", "Weight Log Reminder", "Nhắc nhở cân nặng" },
                    { 4, "WATER_REMINDER", "Nhắc nhở uống nước", "Water Reminder", "Nhắc nhở uống nước" },
                    { 5, "GOAL_ACHIEVED", "Thông báo khi đạt mục tiêu", "Goal Achieved", "Đạt mục tiêu" },
                    { 6, "DAILY_SUMMARY", "Tổng kết dinh dưỡng và tập luyện trong ngày", "Daily Summary", "Tổng kết ngày" },
                    { 7, "WEEKLY_REPORT", "Báo cáo tiến độ hàng tuần", "Weekly Report", "Báo cáo tuần" }
                });

            migrationBuilder.InsertData(
                table: "exercises",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "IconUrl", "MetValue", "NameEn", "NameVi", "Status", "Unit", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), 1, new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2552), null, null, 8.0m, "Running", "Chạy bộ", (byte)1, "minutes", new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2556) },
                    { new Guid("10000000-0000-0000-0000-000000000002"), 1, new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2558), null, null, 3.5m, "Walking", "Đi bộ", (byte)1, "minutes", new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2559) },
                    { new Guid("10000000-0000-0000-0000-000000000003"), 1, new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2561), null, null, 7.5m, "Cycling", "Đạp xe", (byte)1, "minutes", new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2561) },
                    { new Guid("10000000-0000-0000-0000-000000000004"), 1, new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2564), null, null, 9.0m, "Swimming", "Bơi lội", (byte)1, "minutes", new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2564) },
                    { new Guid("10000000-0000-0000-0000-000000000005"), 1, new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2566), null, null, 12.0m, "Jump Rope", "Nhảy dây", (byte)1, "minutes", new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2566) },
                    { new Guid("10000000-0000-0000-0000-000000000006"), 2, new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2569), null, null, 6.0m, "Weight Training", "Tập tạ", (byte)1, "minutes", new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2569) },
                    { new Guid("10000000-0000-0000-0000-000000000007"), 2, new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2571), null, null, 8.0m, "Push-ups", "Hít đất", (byte)1, "reps", new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2572) },
                    { new Guid("10000000-0000-0000-0000-000000000008"), 2, new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2573), null, null, 8.0m, "Sit-ups", "Gập bụng", (byte)1, "reps", new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2573) },
                    { new Guid("10000000-0000-0000-0000-000000000009"), 3, new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2576), null, null, 3.0m, "Yoga", "Yoga", (byte)1, "minutes", new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2576) },
                    { new Guid("10000000-0000-0000-0000-000000000010"), 3, new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2657), null, null, 4.0m, "Pilates", "Pilates", (byte)1, "minutes", new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2658) },
                    { new Guid("10000000-0000-0000-0000-000000000011"), 4, new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2660), null, null, 10.0m, "Football/Soccer", "Bóng đá", (byte)1, "minutes", new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2661) },
                    { new Guid("10000000-0000-0000-0000-000000000012"), 4, new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2662), null, null, 7.0m, "Badminton", "Cầu lông", (byte)1, "minutes", new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2663) },
                    { new Guid("10000000-0000-0000-0000-000000000013"), 4, new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2665), null, null, 8.0m, "Basketball", "Bóng rổ", (byte)1, "minutes", new DateTime(2026, 5, 19, 15, 17, 22, 573, DateTimeKind.Utc).AddTicks(2665) }
                });

            migrationBuilder.CreateIndex(
                name: "idx_exercise_logs_user_date",
                table: "exercise_logs",
                columns: new[] { "UserId", "LogDate" });

            migrationBuilder.CreateIndex(
                name: "IX_exercise_logs_ExerciseId",
                table: "exercise_logs",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "idx_exercise_category",
                table: "exercises",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "idx_exercise_status",
                table: "exercises",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_notification_types_Code",
                table: "notification_types",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_notif_user_created",
                table: "notifications",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "idx_notif_user_read",
                table: "notifications",
                columns: new[] { "UserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_notifications_NotificationTypeId",
                table: "notifications",
                column: "NotificationTypeId");

            migrationBuilder.CreateIndex(
                name: "idx_user_notif_setting_unique",
                table: "user_notification_settings",
                columns: new[] { "UserId", "NotificationTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_notification_settings_NotificationTypeId",
                table: "user_notification_settings",
                column: "NotificationTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exercise_logs");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "user_notification_settings");

            migrationBuilder.DropTable(
                name: "exercises");

            migrationBuilder.DropTable(
                name: "notification_types");

            migrationBuilder.DropTable(
                name: "exercise_categories");
        }
    }
}
