using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Foods;

public class FoodSearchResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name_vi")]
    public string NameVi { get; set; } = null!;

    [JsonPropertyName("name_en")]
    public string? NameEn { get; set; }

    [JsonPropertyName("category_id")]
    public byte CategoryId { get; set; }

    [JsonPropertyName("source")]
    public byte Source { get; set; }

    [JsonPropertyName("serving_size_g")]
    public decimal ServingSizeG { get; set; }

    [JsonPropertyName("serving_unit_vi")]
    public string ServingUnitVi { get; set; } = "g";

    [JsonPropertyName("calories_kcal")]
    public decimal? CaloriesKcal { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }
}
