using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nutrition_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPushToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_push_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Token = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Platform = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_push_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_push_tokens_users_UserId",
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
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9426), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9430) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9435), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9436) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9438), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9439) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9442), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9442) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9445), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9445) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9457), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9458) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9460), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9461) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9464), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9464) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9480), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9480) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9468), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9468) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9483), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9483) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9486), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9486) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9489), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9489) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000014"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9448), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9448) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000015"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9452), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9452) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000016"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9454), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9455) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000017"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9471), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9471) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000018"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9474), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9474) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000019"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9477), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9477) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000020"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9492), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9492) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000021"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9496), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9496) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000022"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9499), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9499) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000023"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9502), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9503) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000024"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9506), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9507) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000025"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9509), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9510) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000026"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9512), new DateTime(2026, 6, 1, 3, 10, 9, 817, DateTimeKind.Utc).AddTicks(9513) });

            migrationBuilder.CreateIndex(
                name: "idx_push_token_unique",
                table: "user_push_tokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_push_tokens_UserId",
                table: "user_push_tokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_push_tokens");

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9448), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9452) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9455), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9455) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9458), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9458) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9461), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9461) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9465), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9465) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9475), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9475) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9477), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9477) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9479), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9480) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9495), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9496) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9485), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9485) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9499), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9499) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9501), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9501) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9503), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9504) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000014"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9468), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9468) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000015"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9470), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9470) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000016"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9472), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9473) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000017"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9488), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9489) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000018"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9491), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9491) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000019"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9493), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9493) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000020"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9537), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9537) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000021"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9539), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9539) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000022"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9541), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9542) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000023"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9544), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9544) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000024"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9546), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9547) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000025"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9549), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9549) });

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000026"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9551), new DateTime(2026, 5, 31, 18, 21, 9, 255, DateTimeKind.Utc).AddTicks(9551) });
        }
    }
}
