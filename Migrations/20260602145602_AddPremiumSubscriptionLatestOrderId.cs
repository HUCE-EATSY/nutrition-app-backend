using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nutrition_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPremiumSubscriptionLatestOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_subscription_events_subscriptions_SubscriptionId",
                table: "subscription_events");

            migrationBuilder.DropIndex(
                name: "IX_subscription_events_SubscriptionId",
                table: "subscription_events");

            migrationBuilder.RenameIndex(
                name: "idx_subscription_user_status",
                table: "subscriptions",
                newName: "idx_sub_user_status");

            migrationBuilder.AddColumn<string>(
                name: "LatestOrderId",
                table: "subscriptions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9466), new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9468) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9472), new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9472) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9477), new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9477) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9481), new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9481) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9483), new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9483) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9485), new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9486) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9488), new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9488) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9490), new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9491) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9493), new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9493) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9495), new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9495) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9497), new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9498) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9500), new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9500) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9502), new DateTime(2026, 6, 2, 14, 56, 1, 609, DateTimeKind.Utc).AddTicks(9502) });

            migrationBuilder.CreateIndex(
                name: "idx_sub_event",
                table: "subscription_events",
                columns: new[] { "SubscriptionId", "ReceivedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_subscription_events_subscriptions_SubscriptionId",
                table: "subscription_events",
                column: "SubscriptionId",
                principalTable: "subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_subscription_events_subscriptions_SubscriptionId",
                table: "subscription_events");

            migrationBuilder.DropIndex(
                name: "idx_sub_event",
                table: "subscription_events");

            migrationBuilder.DropColumn(
                name: "LatestOrderId",
                table: "subscriptions");

            migrationBuilder.RenameIndex(
                name: "idx_sub_user_status",
                table: "subscriptions",
                newName: "idx_subscription_user_status");

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
                name: "IX_subscription_events_SubscriptionId",
                table: "subscription_events",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_subscription_events_subscriptions_SubscriptionId",
                table: "subscription_events",
                column: "SubscriptionId",
                principalTable: "subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
