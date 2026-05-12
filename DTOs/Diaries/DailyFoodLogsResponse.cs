using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Diaries;

public class DailyFoodLogsResponse
{
    [JsonPropertyName("date")]
    public DateOnly Date { get; set; }

    [JsonPropertyName("meals")]
    public List<MealGroupDto> Meals { get; set; } = new();
}

public class MealGroupDto
{
    [JsonPropertyName("meal_type_id")]
    public byte MealTypeId { get; set; }

    [JsonPropertyName("meal_type_name")]
    public string MealTypeName { get; set; } = null!;

    [JsonPropertyName("total_calories")]
    public decimal TotalCalories { get; set; }

    [JsonPropertyName("logs")]
    public List<FoodLogResponse> Logs { get; set; } = new();
}
