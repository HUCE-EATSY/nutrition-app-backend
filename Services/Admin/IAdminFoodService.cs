namespace nutrition_app_backend.Services.Admin;

using nutrition_app_backend.DTOs.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public interface IAdminFoodService
{
    Task<IEnumerable<AdminFoodDto>> GetAllFoodsAsync(int page, int pageSize, string? search, byte? categoryId);
    Task<AdminFoodDto> CreateFoodAsync(AdminFoodCreateDto dto, Guid adminId);
    Task<AdminFoodDto?> UpdateFoodAsync(Guid id, AdminFoodUpdateDto dto);
    Task<bool> DeleteFoodAsync(Guid id);
    Task<bool> ToggleVisibilityAsync(Guid id);
    Task<AdminFoodStatsDto> GetStatsAsync();
    Task<IEnumerable<AdminFoodCategoryDto>> GetCategoriesAsync();
}
