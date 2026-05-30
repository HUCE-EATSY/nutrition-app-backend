using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Diaries;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Enums;
using AutoMapper;
namespace nutrition_app_backend.Services.FoodLog;

public class FoodLogService : IFoodLogService
{
    private readonly WaoDbContext _db;
    private readonly IMapper _mapper;

    public FoodLogService(WaoDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <summary>
    /// Create a food log entry.
    /// - Validates food_item exists and is not rejected (status != 2).
    /// - Validates meal_type_id exists.
    /// - Snapshots macro from food_nutrition at log time using Atwater (4-9-4).
    /// - Scales macros proportionally to quantity_g vs serving_size_g.
    /// </summary>
    public async Task<FoodLogResponse> CreateAsync(Guid userId, CreateFoodLogRequest request)
    {
        // Validate food item
        var food = await _db.FoodItems
            .Include(f => f.Nutrition)
            .Include(f => f.ActiveImage)
            .FirstOrDefaultAsync(f => f.Id == request.FoodItemId);

        if (food == null)
            throw new NotFoundException("Không tìm thấy món ăn.");

        if (food.Status == FoodStatus.Rejected)
            throw new BusinessException("FOOD_REJECTED", "Món ăn này đã bị từ chối, không thể tạo log.");

        // Validate meal type
        var mealType = await _db.MealTypes.FindAsync(request.MealTypeId);
        if (mealType == null)
            throw new BusinessException("INVALID_MEAL_TYPE", "Loại bữa ăn không hợp lệ.");

        if (food.Nutrition == null)
            throw new BusinessException("NO_NUTRITION", "Món ăn chưa có dữ liệu dinh dưỡng.");

        // Snapshot macros scaled to quantity
        var ratio = request.QuantityG / food.ServingSizeG;
        var (calories, protein, carbs, fat) = CalculateSnapshotMacros(food.Nutrition, ratio);

        var log = new Models.Diaries.FoodLog
        {
            UserId = userId,
            FoodItemId = request.FoodItemId,
            MealTypeId = request.MealTypeId,
            LogDate = request.LogDate,
            QuantityG = request.QuantityG,
            CaloriesKcal = calories,
            ProteinG = protein,
            CarbsG = carbs,
            FatG = fat,
            InputMethod = request.InputMethod,
            Note = request.Note,
            CreatedAt = DateTime.UtcNow,
        };

        _db.FoodLogs.Add(log);
        await _db.SaveChangesAsync();

        return _mapper.Map<FoodLogResponse>(log);
    }

    /// <summary>
    /// Update quantity of an existing food log. Recalculates macros.
    /// </summary>
    public async Task<FoodLogResponse> UpdateAsync(Guid userId, ulong logId, UpdateFoodLogRequest request)
    {
        var log = await _db.FoodLogs
            .Include(l => l.FoodItem).ThenInclude(f => f.Nutrition)
            .Include(l => l.FoodItem).ThenInclude(f => f.ActiveImage)
            .Include(l => l.MealType)
            .FirstOrDefaultAsync(l => l.Id == logId);

        if (log == null)
            throw new NotFoundException("Không tìm thấy log.");

        if (log.UserId != userId)
            throw new ForbiddenException("Bạn không có quyền sửa log này.");

        // Recalculate macros with new quantity
        if (log.FoodItem.Nutrition != null)
        {
            var ratio = request.QuantityG / log.FoodItem.ServingSizeG;
            var (calories, protein, carbs, fat) = CalculateSnapshotMacros(log.FoodItem.Nutrition, ratio);

            log.QuantityG = request.QuantityG;
            log.CaloriesKcal = calories;
            log.ProteinG = protein;
            log.CarbsG = carbs;
            log.FatG = fat;
        }
        else
        {
            log.QuantityG = request.QuantityG;
        }

        await _db.SaveChangesAsync();

        return _mapper.Map<FoodLogResponse>(log);
    }

    /// <summary>
    /// Delete a food log. Only the owner can delete.
    /// </summary>
    public async Task DeleteAsync(Guid userId, ulong logId)
    {
        var log = await _db.FoodLogs.FindAsync(logId);

        if (log == null)
            throw new NotFoundException("Không tìm thấy log.");

        if (log.UserId != userId)
            throw new ForbiddenException("Bạn không có quyền xóa log này.");

        _db.FoodLogs.Remove(log);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Get all food logs for a specific date, grouped by meal type.
    /// </summary>
    public async Task<DailyFoodLogsResponse> GetDailyLogsAsync(Guid userId, DateOnly date)
    {
        var logs = await _db.FoodLogs
            .Where(l => l.UserId == userId && l.LogDate == date)
            .Include(l => l.FoodItem).ThenInclude(f => f.ActiveImage)
            .Include(l => l.MealType)
            .OrderBy(l => l.MealTypeId)
            .ThenBy(l => l.CreatedAt)
            .ToListAsync();

        var mealTypes = await _db.MealTypes.ToListAsync();

        var meals = mealTypes
            .Select(mt =>
            {
                var mealLogs = logs.Where(l => l.MealTypeId == mt.Id).ToList();
                return new MealGroupDto
                {
                    MealTypeId = mt.Id,
                    MealTypeName = mt.NameVi,
                    TotalCalories = mealLogs.Sum(l => l.CaloriesKcal),
                    Logs = mealLogs.Select(l => _mapper.Map<FoodLogResponse>(l)).ToList()
                };
            })
            .Where(m => m.Logs.Count > 0) // Only include meals that have logs
            .ToList();

        return new DailyFoodLogsResponse
        {
            Date = date,
            Meals = meals
        };
    }

    /// <summary>
    /// Get daily summary: total macros + comparison with active user goal target.
    /// </summary>
    public async Task<DailySummaryResponse> GetDailySummaryAsync(Guid userId, DateOnly date)
    {
        var logs = await _db.FoodLogs
            .Where(l => l.UserId == userId && l.LogDate == date)
            .ToListAsync();

        var totalCalories = logs.Sum(l => l.CaloriesKcal);
        var totalProtein = logs.Sum(l => l.ProteinG);
        var totalCarbs = logs.Sum(l => l.CarbsG);
        var totalFat = logs.Sum(l => l.FatG);

        // Try to get active goal for target comparison
        var activeGoal = await _db.UserGoals
            .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);

        DailyTargetDto? target = null;
        if (activeGoal != null)
        {
            target = new DailyTargetDto
            {
                TargetCalories = activeGoal.TargetCalories,
                TargetProteinG = activeGoal.TargetProteinG,
                TargetCarbsG = activeGoal.TargetCarbsG,
                TargetFatG = activeGoal.TargetFatG,
                CaloriesPct = activeGoal.TargetCalories > 0
                    ? Math.Round(totalCalories / activeGoal.TargetCalories * 100, 1)
                    : 0,
                ProteinPct = activeGoal.TargetProteinG > 0
                    ? Math.Round(totalProtein / activeGoal.TargetProteinG * 100, 1)
                    : 0,
                CarbsPct = activeGoal.TargetCarbsG > 0
                    ? Math.Round(totalCarbs / activeGoal.TargetCarbsG * 100, 1)
                    : 0,
                FatPct = activeGoal.TargetFatG > 0
                    ? Math.Round(totalFat / activeGoal.TargetFatG * 100, 1)
                    : 0,
            };
        }

        return new DailySummaryResponse
        {
            Date = date,
            TotalCalories = totalCalories,
            TotalProteinG = totalProtein,
            TotalCarbsG = totalCarbs,
            TotalFatG = totalFat,
            Target = target
        };
    }

    // --- Private Helpers ---

    private static (decimal calories, decimal protein, decimal carbs, decimal fat)
        CalculateSnapshotMacros(Models.Foods.FoodNutrition nutrition, decimal ratio)
    {
        var protein = Math.Round(nutrition.ProteinG * ratio, 2);
        var carbs = Math.Round(nutrition.CarbsG * ratio, 2);
        var fat = Math.Round(nutrition.FatG * ratio, 2);
        // Scale the stored calories linearly to match frontend's logic and respect food label calories
        var calories = Math.Round(nutrition.CaloriesKcal * ratio, 2);

        return (calories, protein, carbs, fat);
    }
}
