using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using nutrition_app_backend.DTOs.Foods;

namespace nutrition_app_backend.DTOs.Admin;

public class CreateOfficialFoodRequest
{
    [Required]
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

    [JsonPropertyName("barcode")]
    public ulong? Barcode { get; set; }

    [JsonPropertyName("image")]
    public IFormFile? Image { get; set; }

    // Dùng chung CreateFoodNutritionDto
    [JsonPropertyName("nutrition")]
    public CreateFoodNutritionDto? Nutrition { get; set; }
}

public class UpdateFoodMetadataRequest
{
    [Required]
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
}


public class SetActiveImageRequest
{
    [Required]
    [JsonPropertyName("image_id")]
    public ulong ImageId { get; set; }
}

public class AddComponentRequest
{
    [Required]
    [JsonPropertyName("child_food_id")]
    public Guid ChildFoodId { get; set; }

    [Required]
    [Range(0.01, 9999.99)]
    [JsonPropertyName("quantity_g")]
    public decimal QuantityG { get; set; }
}

public class ReviewCommunityFoodRequest
{
    [Required]
    [JsonPropertyName("approve")]
    public bool Approve { get; set; } // true = status 1, false = status 2
}
