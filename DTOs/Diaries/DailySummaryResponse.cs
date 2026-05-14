using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Diaries;

public class DailySummaryResponse
{
    [JsonPropertyName("date")]
    public DateOnly Date { get; set; }

    [JsonPropertyName("total_calories")]
    public decimal TotalCalories { get; set; }

    [JsonPropertyName("total_protein_g")]
    public decimal TotalProteinG { get; set; }

    [JsonPropertyName("total_carbs_g")]
    public decimal TotalCarbsG { get; set; }

    [JsonPropertyName("total_fat_g")]
    public decimal TotalFatG { get; set; }

    [JsonPropertyName("target")]
    public DailyTargetDto? Target { get; set; }
}

public class DailyTargetDto
{
    [JsonPropertyName("target_calories")]
    public decimal TargetCalories { get; set; }

    [JsonPropertyName("target_protein_g")]
    public decimal TargetProteinG { get; set; }

    [JsonPropertyName("target_carbs_g")]
    public decimal TargetCarbsG { get; set; }

    [JsonPropertyName("target_fat_g")]
    public decimal TargetFatG { get; set; }

    [JsonPropertyName("calories_pct")]
    public decimal CaloriesPct { get; set; }

    [JsonPropertyName("protein_pct")]
    public decimal ProteinPct { get; set; }

    [JsonPropertyName("carbs_pct")]
    public decimal CarbsPct { get; set; }

    [JsonPropertyName("fat_pct")]
    public decimal FatPct { get; set; }
}
