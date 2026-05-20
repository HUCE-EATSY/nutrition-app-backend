using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Foods;

public class FoodComponentResponse
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("child_food_id")]
    public Guid ChildFoodId { get; set; }

    [JsonPropertyName("child_food_name_vi")]
    public string ChildFoodNameVi { get; set; } = null!;

    [JsonPropertyName("child_food_name_en")]
    public string? ChildFoodNameEn { get; set; }

    [JsonPropertyName("quantity_g")]
    public decimal QuantityG { get; set; }

    [JsonPropertyName("calories_kcal")]
    public decimal? CaloriesKcal { get; set; }

    [JsonPropertyName("protein_g")]
    public decimal? ProteinG { get; set; }

    [JsonPropertyName("carbs_g")]
    public decimal? CarbsG { get; set; }

    [JsonPropertyName("fat_g")]
    public decimal? FatG { get; set; }

    [JsonPropertyName("child_food_image_url")]
    public string? ChildFoodImageUrl { get; set; }
}
