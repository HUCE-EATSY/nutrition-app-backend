using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Models.Foods;

namespace nutrition_app_backend.Services.Foods;

public interface IFoodService
{
    Task<IEnumerable<FoodDto>> GetAllAsync(string? category = null, string? search = null);
    Task<FoodDto?> GetByIdAsync(int id);
    Task<FoodDto> CreateAsync(CreateFoodDto dto);
    Task<FoodDto?> UpdateAsync(int id, UpdateFoodDto dto);
    Task<bool> DeleteAsync(int id);
}
