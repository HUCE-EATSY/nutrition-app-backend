using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Foods;

public class CreateFoodRequest
{
    [Required(ErrorMessage = "Tên tiếng Việt là bắt buộc.")]
    [MaxLength(200)]
    [JsonPropertyName("name_vi")]
    public string NameVi { get; set; } = null!;

    [MaxLength(200)]
    [JsonPropertyName("name_en")]
    public string? NameEn { get; set; }

    [Required]
    [JsonPropertyName("category_id")]
    public byte CategoryId { get; set; }

    [Required]
    [Range(0.01, 99999.99)]
    [JsonPropertyName("serving_size_g")]
    public decimal ServingSizeG { get; set; }

    [MaxLength(50)]
    [JsonPropertyName("serving_unit_vi")]
    public string ServingUnitVi { get; set; } = "g";

    [MaxLength(500)]
    [JsonPropertyName("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }

    [JsonPropertyName("barcode")]
    public ulong? Barcode { get; set; }

    [Required]
    [JsonPropertyName("nutrition")]
    public CreateFoodNutritionDto Nutrition { get; set; } = null!;
}

public class CreateFoodNutritionDto
{
    [Required]
    [Range(0, 99999.99)]
    [JsonPropertyName("calories_kcal")]
    public decimal CaloriesKcal { get; set; }

    [Range(0, 9999.99)]
    [JsonPropertyName("protein_g")]
    public decimal ProteinG { get; set; }

    [Range(0, 9999.99)]
    [JsonPropertyName("carbs_g")]
    public decimal CarbsG { get; set; }

    [Range(0, 9999.99)]
    [JsonPropertyName("fat_g")]
    public decimal FatG { get; set; }

    [Range(0, 9999.99)]
    [JsonPropertyName("fiber_g")]
    public decimal? FiberG { get; set; }

    [Range(0, 9999.99)]
    [JsonPropertyName("sugar_g")]
    public decimal? SugarG { get; set; }

    [Range(0, 99999.99)]
    [JsonPropertyName("sodium_mg")]
    public decimal? SodiumMg { get; set; }
}
