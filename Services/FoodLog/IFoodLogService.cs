using nutrition_app_backend.DTOs.Diaries;

namespace nutrition_app_backend.Services.FoodLog;

public interface IFoodLogService
{
    Task<FoodLogResponse> CreateAsync(Guid userId, CreateFoodLogRequest request);
    Task<FoodLogResponse> UpdateAsync(Guid userId, ulong logId, UpdateFoodLogRequest request);
    Task DeleteAsync(Guid userId, ulong logId);
    Task<DailyFoodLogsResponse> GetDailyLogsAsync(Guid userId, DateOnly date);
    Task<DailySummaryResponse> GetDailySummaryAsync(Guid userId, DateOnly date);
    Task<List<DailySummaryResponse>> GetTimelineSummaryAsync(Guid userId, DateOnly from, DateOnly to);
}
