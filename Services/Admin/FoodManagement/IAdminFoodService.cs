using nutrition_app_backend.DTOs.Admin;
using nutrition_app_backend.DTOs.Foods;

namespace nutrition_app_backend.Services.Admin.FoodManagement;

public interface IAdminFoodService
{
    Task<FoodDetailResponse> CreateOfficialFoodAsync(Guid adminId, CreateOfficialFoodRequest request);
    Task<FoodDetailResponse> UpdateFoodMetadataAsync(Guid adminId, Guid foodId, UpdateFoodMetadataRequest request);
    Task DeleteFoodAsync(Guid adminId, Guid foodId);
    
    Task<FoodDetailResponse> AddOrUpdateNutritionAsync(Guid adminId, Guid foodId, CreateFoodNutritionDto request);
    
    Task<object> UploadFoodImageAsync(Guid adminId, Guid foodId, IFormFile image);
    Task<FoodDetailResponse> SetActiveImageAsync(Guid adminId, Guid foodId, SetActiveImageRequest request);
    Task DeleteImageAsync(Guid adminId, Guid foodId, ulong imageId);
    
    Task<FoodDetailResponse> AddComponentAsync(Guid adminId, Guid foodId, AddComponentRequest request);
    Task DeleteComponentAsync(Guid adminId, Guid foodId, ulong componentId);
    
    Task<PaginatedResponse<FoodSearchResponse>> GetPendingFoodsAsync(int page, int pageSize);
    Task<FoodDetailResponse> ReviewCommunityFoodAsync(Guid adminId, Guid foodId, ReviewCommunityFoodRequest request);
}
