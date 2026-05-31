namespace nutrition_app_backend.Services.Admin;

using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Admin;
using nutrition_app_backend.Enums;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Models.Foods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AdminFoodService : IAdminFoodLegacyService
{
    private readonly WaoDbContext _dbContext;

    public AdminFoodService(WaoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<AdminFoodDto>> GetAllFoodsAsync(int page, int pageSize, string? search, byte? categoryId)
    {
        var query = _dbContext.FoodItems.Include(f => f.Nutrition).AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(f => f.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(f => f.NameVi.ToLower().Contains(searchLower) || (f.NameEn != null && f.NameEn.ToLower().Contains(searchLower)));
        }

        var foods = await query
            .OrderByDescending(f => f.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new AdminFoodDto
            {
                Id = f.Id,
                NameVi = f.NameVi,
                NameEn = f.NameEn,
                CategoryId = f.CategoryId,
                Status = (byte)f.Status,
                ServingSizeG = f.ServingSizeG,
                ServingUnitVi = f.ServingUnitVi,
                ThumbnailUrl = f.ThumbnailUrl,
                CreatedAt = f.CreatedAt,
                Nutrition = f.Nutrition != null ? new AdminNutritionDto
                {
                    CaloriesKcal = f.Nutrition.CaloriesKcal,
                    ProteinG = f.Nutrition.ProteinG,
                    CarbsG = f.Nutrition.CarbsG,
                    FatG = f.Nutrition.FatG
                } : null
            })
            .ToListAsync();

        return foods;
    }

    public async Task<AdminFoodDto> CreateFoodAsync(AdminFoodCreateDto dto, Guid adminId)
    {
        var food = new FoodItem
        {
            Id = Guid.NewGuid(),
            NameVi = dto.NameVi,
            NameEn = dto.NameEn,
            CategoryId = dto.CategoryId,
            Source = FoodSource.Official, // Admin source
            Status = FoodStatus.Approved, // Active
            ServingSizeG = dto.ServingSizeG,
            ServingUnitVi = dto.ServingUnitVi ?? "g",
            ThumbnailUrl = dto.ThumbnailUrl,
            CreatedBy = adminId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Nutrition = new FoodNutrition
            {
                CaloriesKcal = dto.Nutrition.CaloriesKcal,
                ProteinG = dto.Nutrition.ProteinG,
                CarbsG = dto.Nutrition.CarbsG,
                FatG = dto.Nutrition.FatG,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _dbContext.FoodItems.Add(food);
        await _dbContext.SaveChangesAsync();

        return new AdminFoodDto
        {
            Id = food.Id,
            NameVi = food.NameVi,
            NameEn = food.NameEn,
            CategoryId = food.CategoryId,
            Status = (byte)food.Status,
            ServingSizeG = food.ServingSizeG,
            ServingUnitVi = food.ServingUnitVi,
            ThumbnailUrl = food.ThumbnailUrl,
            CreatedAt = food.CreatedAt,
            Nutrition = new AdminNutritionDto
            {
                CaloriesKcal = food.Nutrition.CaloriesKcal,
                ProteinG = food.Nutrition.ProteinG,
                CarbsG = food.Nutrition.CarbsG,
                FatG = food.Nutrition.FatG
            }
        };
    }

    public async Task<AdminFoodDto?> UpdateFoodAsync(Guid id, AdminFoodUpdateDto dto)
    {
        var food = await _dbContext.FoodItems.Include(f => f.Nutrition).FirstOrDefaultAsync(f => f.Id == id);
        if (food == null) throw new NotFoundException("Food not found.");

        if (dto.NameVi != null) food.NameVi = dto.NameVi;
        if (dto.NameEn != null) food.NameEn = dto.NameEn;
        if (dto.CategoryId.HasValue) food.CategoryId = dto.CategoryId.Value;
        if (dto.ServingSizeG.HasValue) food.ServingSizeG = dto.ServingSizeG.Value;
        if (dto.ServingUnitVi != null) food.ServingUnitVi = dto.ServingUnitVi;
        if (dto.ThumbnailUrl != null) food.ThumbnailUrl = dto.ThumbnailUrl;
        
        if (dto.Nutrition != null)
        {
            if (food.Nutrition == null)
            {
                food.Nutrition = new FoodNutrition { FoodItemId = id, UpdatedAt = DateTime.UtcNow };
                _dbContext.FoodNutritions.Add(food.Nutrition);
            }
            food.Nutrition.CaloriesKcal = dto.Nutrition.CaloriesKcal;
            food.Nutrition.ProteinG = dto.Nutrition.ProteinG;
            food.Nutrition.CarbsG = dto.Nutrition.CarbsG;
            food.Nutrition.FatG = dto.Nutrition.FatG;
            food.Nutrition.UpdatedAt = DateTime.UtcNow;
        }

        food.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return new AdminFoodDto
        {
            Id = food.Id,
            NameVi = food.NameVi,
            NameEn = food.NameEn,
            CategoryId = food.CategoryId,
            Status = (byte)food.Status,
            ServingSizeG = food.ServingSizeG,
            ServingUnitVi = food.ServingUnitVi,
            ThumbnailUrl = food.ThumbnailUrl,
            CreatedAt = food.CreatedAt,
            Nutrition = food.Nutrition != null ? new AdminNutritionDto
            {
                CaloriesKcal = food.Nutrition.CaloriesKcal,
                ProteinG = food.Nutrition.ProteinG,
                CarbsG = food.Nutrition.CarbsG,
                FatG = food.Nutrition.FatG
            } : null
        };
    }

    public async Task<bool> DeleteFoodAsync(Guid id)
    {
        var food = await _dbContext.FoodItems.FindAsync(id);
        if (food == null) throw new NotFoundException("Food not found.");

        _dbContext.FoodItems.Remove(food);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleVisibilityAsync(Guid id)
    {
        var food = await _dbContext.FoodItems.FindAsync(id);
        if (food == null) throw new NotFoundException("Food not found.");

        food.Status = food.Status == FoodStatus.Approved ? FoodStatus.Pending : FoodStatus.Approved;
        food.UpdatedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<AdminFoodStatsDto> GetStatsAsync()
    {
        var total = await _dbContext.FoodItems.CountAsync();
        var visible = await _dbContext.FoodItems.CountAsync(f => f.Status == FoodStatus.Approved);
        var categories = await _dbContext.FoodItems.Select(f => f.CategoryId).Distinct().CountAsync();

        return new AdminFoodStatsDto
        {
            Total = total,
            Visible = visible,
            Hidden = total - visible,
            Categories = categories
        };
    }

    public async Task<IEnumerable<AdminFoodCategoryDto>> GetCategoriesAsync()
    {
        // Category name mapping (based on common nutrition app categories)
        var categoryNames = new Dictionary<byte, string>
        {
            { 1, "Ngũ cốc & Tinh bột" },
            { 2, "Rau củ" },
            { 3, "Trái cây" },
            { 4, "Thịt & Hải sản" },
            { 5, "Sữa & Trứng" },
            { 6, "Đậu & Hạt" },
            { 7, "Dầu & Chất béo" },
            { 8, "Gia vị & Nước chấm" },
            { 9, "Đồ uống" },
            { 10, "Thức ăn nhanh" },
            { 11, "Bánh & Kẹo" },
            { 12, "Khác" }
        };

        var grouped = await _dbContext.FoodItems
            .GroupBy(f => f.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToListAsync();

        return grouped.Select(g => new AdminFoodCategoryDto
        {
            Id = g.CategoryId,
            Name = categoryNames.ContainsKey(g.CategoryId) ? categoryNames[g.CategoryId] : $"Danh mục {g.CategoryId}",
            FoodCount = g.Count
        }).OrderBy(c => c.Id);
    }
}

