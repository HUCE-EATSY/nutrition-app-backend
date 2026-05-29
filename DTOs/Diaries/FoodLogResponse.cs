using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Diaries;

public class FoodLogResponse
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("food_item_id")]
    public Guid FoodItemId { get; set; }

    [JsonPropertyName("food_name_vi")]
    public string FoodNameVi { get; set; } = null!;

    [JsonPropertyName("food_name_en")]
    public string? FoodNameEn { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("meal_type_id")]
    public byte MealTypeId { get; set; }

    [JsonPropertyName("meal_type_name")]
    public string MealTypeName { get; set; } = null!;

    [JsonPropertyName("log_date")]
    public DateTime LogDate { get; set; }

    [JsonPropertyName("quantity_g")]
    public decimal QuantityG { get; set; }

    [JsonPropertyName("calories_kcal")]
    public decimal CaloriesKcal { get; set; }

    [JsonPropertyName("protein_g")]
    public decimal ProteinG { get; set; }

    [JsonPropertyName("carbs_g")]
    public decimal CarbsG { get; set; }

    [JsonPropertyName("fat_g")]
    public decimal FatG { get; set; }

    [JsonPropertyName("input_method")]
    public byte InputMethod { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}
