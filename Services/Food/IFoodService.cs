using nutrition_app_backend.DTOs.Foods;

namespace nutrition_app_backend.Services.Food;

public interface IFoodService
{
    Task<PaginatedResponse<FoodSearchResponse>> GetListAsync(FoodListRequest request, Guid? currentUserId);
    Task<PaginatedResponse<FoodSearchResponse>> SearchAsync(FoodSearchRequest request, Guid? currentUserId);
    Task<FoodDetailResponse> GetByIdAsync(Guid id);
    Task<List<FoodComponentResponse>> GetComponentsAsync(Guid foodItemId);
    Task<FoodDetailResponse> GetByBarcodeAsync(ulong barcode);
    Task<FoodDetailResponse> CreateAsync(CreateFoodRequest request, Guid userId);
    Task<FoodDetailResponse> CreateRecipeAsync(CreateRecipeRequest request, Guid userId);
    Task<List<MealTypeResponse>> GetMealTypesAsync();
}
