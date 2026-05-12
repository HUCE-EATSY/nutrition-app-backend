using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nutrition_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddDiaryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "diary_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FoodId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FoodName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateISO = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hour = table.Column<int>(type: "int", nullable: false),
                    QuantityG = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    TotalCalories = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    ProteinGram = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    CarbGram = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    FatGram = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_diary_entries", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "exercise_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    ActivityId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActivityLabel = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateISO = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hour = table.Column<int>(type: "int", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    CaloriesBurned = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_logs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_diary_entries_UserId_DateISO",
                table: "diary_entries",
                columns: new[] { "UserId", "DateISO" });

            migrationBuilder.CreateIndex(
                name: "IX_exercise_logs_UserId_DateISO",
                table: "exercise_logs",
                columns: new[] { "UserId", "DateISO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "diary_entries");
            migrationBuilder.DropTable(name: "exercise_logs");
        }
    }
}
