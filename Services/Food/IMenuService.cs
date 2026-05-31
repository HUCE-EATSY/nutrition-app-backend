using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Models.Foods;

namespace nutrition_app_backend.Services.Food;

public interface IMenuService
{
    Task<List<MenuResponse>> GetUserMenusAsync(Guid userId);
    Task<MenuResponse?> GetMenuByIdAsync(Guid menuId, Guid userId);
    Task<MenuResponse> CreateMenuAsync(Guid userId, CreateMenuRequest request);
    Task<MenuResponse> UpdateMenuAsync(Guid menuId, Guid userId, UpdateMenuRequest request);
    Task DeleteMenuAsync(Guid menuId, Guid userId);
    
    Task<MenuResponse> AddFoodToMenuAsync(Guid menuId, Guid userId, AddFoodToMenuRequest request);
    Task<MenuResponse> RemoveFoodFromMenuAsync(Guid menuId, Guid foodItemId, Guid userId);
}
