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
    public string? ServingUnitVi { get; set; } = "g";

    /// Ảnh (tuỳ chọn). Backend sẽ upload lên Cloudinary và tự build URL.
    [JsonPropertyName("image")]
    public IFormFile? Image { get; set; }

    /// URL ảnh đã upload sẵn lên Cloudinary (dùng cho flow nhận diện AI).
    /// Nếu cả Image và ImageUrl đều được cung cấp, Image được ưu tiên.
    [MaxLength(500)]
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [MaxLength(50)]
    [JsonPropertyName("barcode")]
    public string? Barcode { get; set; }

    [Required]
    [JsonPropertyName("nutrition")]
    public CreateFoodNutritionDto Nutrition { get; set; } = null!;
}

public class CreateFoodNutritionDto
{
    [Required(ErrorMessage = "Calories là bắt buộc.")]
    [Range(0, 99999.99)]
    [JsonPropertyName("calories_kcal")]
    public decimal CaloriesKcal { get; set; }

    [Required(ErrorMessage = "Protein là bắt buộc.")]
    [Range(0, 9999.99)]
    [JsonPropertyName("protein_g")]
    public decimal ProteinG { get; set; }

    [Required(ErrorMessage = "Carbs là bắt buộc.")]
    [Range(0, 9999.99)]
    [JsonPropertyName("carbs_g")]
    public decimal CarbsG { get; set; }

    [Required(ErrorMessage = "Chất béo là bắt buộc.")]
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
