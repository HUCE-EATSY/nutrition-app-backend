using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Models.Foods;

namespace nutrition_app_backend.Services.Food;

public class MenuService : IMenuService
{
    private readonly WaoDbContext _context;

    public MenuService(WaoDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuResponse>> GetUserMenusAsync(Guid userId)
    {
        var menus = await _context.Menus
            .Include(m => m.MenuFoods)
                .ThenInclude(mf => mf.FoodItem)
                    .ThenInclude(fi => fi.Nutrition)
            .Include(m => m.MenuFoods)
                .ThenInclude(mf => mf.FoodItem)
                    .ThenInclude(fi => fi.ActiveImage)
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        return menus.Select(MapToMenuResponse).ToList();
    }

    public async Task<MenuResponse?> GetMenuByIdAsync(Guid menuId, Guid userId)
    {
        var menu = await _context.Menus
            .Include(m => m.MenuFoods)
                .ThenInclude(mf => mf.FoodItem)
                    .ThenInclude(fi => fi.Nutrition)
            .Include(m => m.MenuFoods)
                .ThenInclude(mf => mf.FoodItem)
                    .ThenInclude(fi => fi.ActiveImage)
            .FirstOrDefaultAsync(m => m.Id == menuId && m.UserId == userId);

        if (menu == null) return null;

        return MapToMenuResponse(menu);
    }

    public async Task<MenuResponse> CreateMenuAsync(Guid userId, CreateMenuRequest request)
    {
        var menu = new Menu
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Menus.Add(menu);
        await _context.SaveChangesAsync();

        return MapToMenuResponse(menu);
    }

    public async Task<MenuResponse> UpdateMenuAsync(Guid menuId, Guid userId, UpdateMenuRequest request)
    {
        var menu = await _context.Menus
            .Include(m => m.MenuFoods)
                .ThenInclude(mf => mf.FoodItem)
                    .ThenInclude(fi => fi.Nutrition)
            .Include(m => m.MenuFoods)
                .ThenInclude(mf => mf.FoodItem)
                    .ThenInclude(fi => fi.ActiveImage)
            .FirstOrDefaultAsync(m => m.Id == menuId && m.UserId == userId);

        if (menu == null)
            throw new NotFoundException("Không tìm thấy thực đơn.");

        menu.Name = request.Name;
        menu.Description = request.Description;
        menu.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToMenuResponse(menu);
    }

    public async Task DeleteMenuAsync(Guid menuId, Guid userId)
    {
        var menu = await _context.Menus.FirstOrDefaultAsync(m => m.Id == menuId && m.UserId == userId);
        if (menu == null)
            throw new NotFoundException("Không tìm thấy thực đơn.");

        _context.Menus.Remove(menu);
        await _context.SaveChangesAsync();
    }

    public async Task<MenuResponse> AddFoodToMenuAsync(Guid menuId, Guid userId, AddFoodToMenuRequest request)
    {
        var menu = await _context.Menus
            .Include(m => m.MenuFoods)
                .ThenInclude(mf => mf.FoodItem)
                    .ThenInclude(fi => fi.Nutrition)
            .Include(m => m.MenuFoods)
                .ThenInclude(mf => mf.FoodItem)
                    .ThenInclude(fi => fi.ActiveImage)
            .FirstOrDefaultAsync(m => m.Id == menuId && m.UserId == userId);

        if (menu == null)
            throw new NotFoundException("Không tìm thấy thực đơn.");

        var food = await _context.FoodItems.FirstOrDefaultAsync(f => f.Id == request.FoodItemId);
        if (food == null)
            throw new NotFoundException("Không tìm thấy món ăn.");

        var existingMenuFood = menu.MenuFoods.FirstOrDefault(mf => mf.FoodItemId == request.FoodItemId);
        if (existingMenuFood != null)
        {
            existingMenuFood.QuantityG += request.QuantityG;
        }
        else
        {
            menu.MenuFoods.Add(new MenuFood
            {
                Id = Guid.NewGuid(),
                MenuId = menuId,
                FoodItemId = request.FoodItemId,
                QuantityG = request.QuantityG
            });
        }

        menu.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Reload the menu to get updated nested items for the response
        return await GetMenuByIdAsync(menuId, userId) ?? MapToMenuResponse(menu);
    }

    public async Task<MenuResponse> RemoveFoodFromMenuAsync(Guid menuId, Guid foodItemId, Guid userId)
    {
        var menu = await _context.Menus
            .Include(m => m.MenuFoods)
            .FirstOrDefaultAsync(m => m.Id == menuId && m.UserId == userId);

        if (menu == null)
            throw new NotFoundException("Không tìm thấy thực đơn.");

        var menuFood = menu.MenuFoods.FirstOrDefault(mf => mf.FoodItemId == foodItemId);
        if (menuFood == null)
            throw new NotFoundException("Món ăn không tồn tại trong thực đơn này.");

        _context.MenuFoods.Remove(menuFood);
        menu.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        return await GetMenuByIdAsync(menuId, userId) ?? throw new Exception("Lỗi load lại thực đơn.");
    }

    private MenuResponse MapToMenuResponse(Menu menu)
    {
        var response = new MenuResponse
        {
            Id = menu.Id,
            Name = menu.Name,
            Description = menu.Description,
            CreatedAt = menu.CreatedAt,
            UpdatedAt = menu.UpdatedAt,
            Foods = new List<MenuFoodResponse>()
        };

        decimal totalProteinG = 0;
        decimal totalCarbsG = 0;
        decimal totalFatG = 0;
        decimal totalCalories = 0;

        foreach (var mf in menu.MenuFoods)
        {
            var food = mf.FoodItem;
            if (food.Nutrition == null) continue;

            // Tính tỷ lệ theo ServingSizeG của món ăn gốc
            decimal ratio = food.ServingSizeG > 0 ? (mf.QuantityG / food.ServingSizeG) : 1;

            var kcal = food.Nutrition.CaloriesKcal * ratio;
            var prot = food.Nutrition.ProteinG * ratio;
            var carb = food.Nutrition.CarbsG * ratio;
            var fat = food.Nutrition.FatG * ratio;

            totalCalories += kcal;
            totalProteinG += prot;
            totalCarbsG += carb;
            totalFatG += fat;

            response.Foods.Add(new MenuFoodResponse
            {
                Id = mf.Id,
                FoodItemId = mf.FoodItemId,
                FoodNameVi = food.NameVi,
                FoodNameEn = food.NameEn,
                ThumbnailUrl = food.ActiveImage?.StoragePath ?? food.ThumbnailUrl,
                QuantityG = mf.QuantityG,
                CaloriesKcal = Math.Round(kcal, 2),
                ProteinG = Math.Round(prot, 2),
                CarbsG = Math.Round(carb, 2),
                FatG = Math.Round(fat, 2)
            });
        }

        response.TotalCalories = Math.Round(totalCalories, 2);
        response.TotalProteinG = Math.Round(totalProteinG, 2);
        response.TotalCarbsG = Math.Round(totalCarbsG, 2);
        response.TotalFatG = Math.Round(totalFatG, 2);

        // Tính % (1g protein = 4kcal, 1g carb = 4kcal, 1g fat = 9kcal)
        decimal kcalFromProtein = totalProteinG * 4;
        decimal kcalFromCarbs = totalCarbsG * 4;
        decimal kcalFromFat = totalFatG * 9;
        
        decimal macroTotalKcal = kcalFromProtein + kcalFromCarbs + kcalFromFat;
        
        if (macroTotalKcal > 0)
        {
            response.ProteinPercentage = Math.Round((kcalFromProtein / macroTotalKcal) * 100, 1);
            response.CarbsPercentage = Math.Round((kcalFromCarbs / macroTotalKcal) * 100, 1);
            response.FatPercentage = Math.Round((kcalFromFat / macroTotalKcal) * 100, 1);
        }
        else
        {
            response.ProteinPercentage = 0;
            response.CarbsPercentage = 0;
            response.FatPercentage = 0;
        }

        return response;
    }
}
