using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Foods;

/// <summary>
/// Returned by POST /api/foods/estimate-nutrients.
/// Contains the Spoonacular-estimated nutrition pre-filled so the frontend
/// can display it in FoodDetailModal and submit directly to POST /api/foods.
/// </summary>
public class EstimatedFoodResponse
{
    /// <summary>Food category / name returned by Spoonacular (e.g. "Fast Food").</summary>
    [JsonPropertyName("name_en")]
    public string NameEn { get; set; } = null!;

    /// <summary>The transformed Cloudinary URL (.jpg) sent to Spoonacular.</summary>
    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = null!;

    /// <summary>Always 100 g — Spoonacular estimates per visible portion, normalised to 100 g.</summary>
    [JsonPropertyName("serving_size_g")]
    public decimal ServingSizeG { get; set; } = 100m;

    /// <summary>
    /// Pre-filled nutrition DTO.
    /// Reuses CreateFoodNutritionDto so the frontend can post it directly to POST /api/foods.
    /// </summary>
    [JsonPropertyName("nutrition")]
    public CreateFoodNutritionDto Nutrition { get; set; } = null!;
}
