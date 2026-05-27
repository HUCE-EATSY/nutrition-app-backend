using nutrition_app_backend.DTOs.Foods;

namespace nutrition_app_backend.Services.Spoonacular;

public interface ISpoonacularService
{
    /// <summary>
    /// Transforms the Cloudinary imageUrl (.webp → .jpg), calls Spoonacular's
    /// estimateNutrients endpoint, and returns a pre-filled EstimatedFoodResponse.
    /// Returns null if Spoonacular cannot identify the food.
    /// </summary>
    Task<EstimatedFoodResponse?> EstimateNutrientsAsync(string imageUrl);
}
