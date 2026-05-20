using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Foods;

public class CreateRecipeRequest
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

    [MaxLength(50)]
    [JsonPropertyName("serving_unit_vi")]
    public string ServingUnitVi { get; set; } = "phần";

    /// Ảnh (tuỳ chọn). Backend sẽ upload lên Cloudinary và tự build URL.
    [JsonPropertyName("image")]
    public IFormFile? Image { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Công thức phải có ít nhất 1 thành phần.")]
    [JsonPropertyName("components")]
    public List<RecipeComponentDto> Components { get; set; } = new();
}

public class RecipeComponentDto
{
    [Required]
    [JsonPropertyName("child_food_id")]
    public Guid ChildFoodId { get; set; }

    [Required]
    [Range(0.01, 9999.99)]
    [JsonPropertyName("quantity_g")]
    public decimal QuantityG { get; set; }
}
