namespace nutrition_app_backend.Services.Admin;

using nutrition_app_backend.DTOs.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

/// <summary>
/// Legacy admin food service interface (kept for backward compatibility with old AdminFoodService).
/// The active interface used by AdminFoodsController is in Services.Admin.FoodManagement namespace.
/// </summary>
public interface IAdminFoodLegacyService
{
    Task<IEnumerable<AdminFoodDto>> GetAllFoodsAsync(int page, int pageSize, string? search, byte? categoryId);
    Task<AdminFoodDto> CreateFoodAsync(AdminFoodCreateDto dto, Guid adminId);
    Task<AdminFoodDto?> UpdateFoodAsync(Guid id, AdminFoodUpdateDto dto);
    Task<bool> DeleteFoodAsync(Guid id);
    Task<bool> ToggleVisibilityAsync(Guid id);
    Task<AdminFoodStatsDto> GetStatsAsync();
    Task<IEnumerable<AdminFoodCategoryDto>> GetCategoriesAsync();
}
