using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Foods;

public class FoodDetailResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name_vi")]
    public string NameVi { get; set; } = null!;

    [JsonPropertyName("name_en")]
    public string? NameEn { get; set; }

    [JsonPropertyName("category")]
    public FoodCategoryDto? Category { get; set; }

    [JsonPropertyName("source")]
    public byte Source { get; set; }

    [JsonPropertyName("status")]
    public byte Status { get; set; }

    [JsonPropertyName("serving_size_g")]
    public decimal ServingSizeG { get; set; }

    [JsonPropertyName("serving_unit_vi")]
    public string ServingUnitVi { get; set; } = "g";

    [JsonPropertyName("barcode")]
    public string? Barcode { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("nutrition")]
    public FoodNutritionDto? Nutrition { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}

public class FoodCategoryDto
{
    [JsonPropertyName("id")]
    public byte Id { get; set; }

    [JsonPropertyName("name_vi")]
    public string NameVi { get; set; } = null!;

    [JsonPropertyName("name_en")]
    public string? NameEn { get; set; }
}

public class FoodNutritionDto
{
    [JsonPropertyName("calories_kcal")]
    public decimal CaloriesKcal { get; set; }

    [JsonPropertyName("protein_g")]
    public decimal ProteinG { get; set; }

    [JsonPropertyName("carbs_g")]
    public decimal CarbsG { get; set; }

    [JsonPropertyName("fat_g")]
    public decimal FatG { get; set; }

    [JsonPropertyName("fiber_g")]
    public decimal? FiberG { get; set; }

    [JsonPropertyName("sugar_g")]
    public decimal? SugarG { get; set; }

    [JsonPropertyName("sodium_mg")]
    public decimal? SodiumMg { get; set; }
}
