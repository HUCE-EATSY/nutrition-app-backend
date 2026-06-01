using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace nutrition_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddStreakAndSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "streak_freeze_transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FreezeDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Source = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_streak_freeze_transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_streak_freeze_transactions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subscription_plans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_plans", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_streaks",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    CurrentStreak = table.Column<int>(type: "int", nullable: false),
                    LongestStreak = table.Column<int>(type: "int", nullable: false),
                    FreezeCount = table.Column<int>(type: "int", nullable: false),
                    LastLogDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_streaks", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_user_streaks_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    CurrentPeriodEnd = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    StoreTransactionId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subscriptions_subscription_plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "subscription_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_subscriptions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subscription_events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    SubscriptionId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    Provider = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EventType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RawPayload = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReceivedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subscription_events_subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.InsertData(
                table: "subscription_plans",
                columns: new[] { "Id", "Code", "CreatedAt", "DurationDays", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "FREE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 99999, "Gói Miễn Phí", 0m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "MONTHLY_PREMIUM", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 30, "Premium 1 Tháng", 59000m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "YEARLY_PREMIUM", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 365, "Premium 1 Năm", 499000m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "idx_freeze_user_date",
                table: "streak_freeze_transactions",
                columns: new[] { "UserId", "FreezeDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscription_events_SubscriptionId",
                table: "subscription_events",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "idx_subscription_user_status",
                table: "subscriptions",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_PlanId",
                table: "subscriptions",
                column: "PlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "streak_freeze_transactions");

            migrationBuilder.DropTable(
                name: "subscription_events");

            migrationBuilder.DropTable(
                name: "user_streaks");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "subscription_plans");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3962), new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3966) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3970), new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3971) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3975), new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3976) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3979), new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3980) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3983), new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3984) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3987), new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3988) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3991), new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3992) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3996), new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(3996) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(4000), new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(4000) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(4004), new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(4004) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(4032), new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(4032) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(4036), new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(4037) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(4041), new DateTime(2026, 5, 27, 2, 58, 21, 765, DateTimeKind.Utc).AddTicks(4041) });
        }
    }
}
