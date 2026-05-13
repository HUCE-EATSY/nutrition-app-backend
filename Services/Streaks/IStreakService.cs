using nutrition_app_backend.Models.Streaks;

namespace nutrition_app_backend.Services.Streaks;

public interface IStreakService
{
    Task<UserStreak?> GetStreakAsync(Guid userId);
    Task<bool> FreezeStreakAsync(Guid userId, DateOnly targetDate);
}
