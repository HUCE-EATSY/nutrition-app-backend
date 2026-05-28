using nutrition_app_backend.DTOs.Foods;

namespace nutrition_app_backend.Services.Food;

public interface IFoodService
{
    Task<CursorPaginatedResponse<FoodSearchResponse>> GetListAsync(FoodListRequest request, Guid? currentUserId);
    Task<PaginatedResponse<FoodSearchResponse>> SearchAsync(FoodSearchRequest request, Guid? currentUserId);
    Task<FoodDetailResponse> GetByIdAsync(Guid id);
    Task<List<FoodComponentResponse>> GetComponentsAsync(Guid foodItemId);
    Task<FoodDetailResponse?> GetByBarcodeAsync(string barcode);

    /// <summary>
    /// Calls Spoonacular's estimateNutrients API using the provided Cloudinary image URL.
    /// Returns a pre-filled EstimatedFoodResponse for the frontend to review and submit.
    /// Returns null if the food cannot be identified.
    /// </summary>
    Task<EstimatedFoodResponse?> EstimateNutrientsFromImageAsync(IFormFile image);

    Task<FoodDetailResponse> CreateAsync(CreateFoodRequest request, Guid userId);
    Task<FoodDetailResponse> CreateRecipeAsync(CreateRecipeRequest request, Guid userId);
    Task<FoodDetailResponse> UpdateAsync(Guid id, CreateFoodRequest request, Guid userId);
    Task<FoodDetailResponse> UpdateRecipeAsync(Guid id, CreateRecipeRequest request, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<List<MealTypeResponse>> GetMealTypesAsync();
}
