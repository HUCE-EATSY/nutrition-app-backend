using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Users;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Services.Streak;

public class StreakService : IStreakService
{
    private readonly WaoDbContext _context;

    public StreakService(WaoDbContext context)
    {
        _context = context;
    }

    public async Task<StreakResponse> GetStreakAsync(Guid userId)
    {
        var streak = await _context.UserStreaks
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (streak == null)
        {
            return new StreakResponse
            {
                CurrentStreak = 0,
                LongestStreak = 0,
                FreezeCount = 0,
                LastLogDate = null
            };
        }

        return new StreakResponse
        {
            CurrentStreak = streak.CurrentStreak,
            LongestStreak = streak.LongestStreak,
            FreezeCount = streak.FreezeCount,
            LastLogDate = streak.LastLogDate
        };
    }

    public async Task<bool> FreezeStreakAsync(Guid userId)
    {
        var streak = await _context.UserStreaks
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (streak == null || streak.FreezeCount <= 0)
        {
            throw new BusinessException("STREAK_NO_FREEZES", "No freeze counts available.");
        }

        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        // Check if already frozen
        var alreadyFrozen = await _context.StreakFreezeTransactions
            .AnyAsync(t => t.UserId == userId && t.ProtectedDate == yesterday);

        if (alreadyFrozen)
        {
            throw new BusinessException("STREAK_ALREADY_FROZEN", "Yesterday is already protected by a freeze.");
        }

        // Check if yesterday has valid logs (calories > 50% of BMR)
        var goal = await _context.UserGoals
            .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);

        decimal bmr = goal?.BmrKcal ?? 0;
        decimal limit = bmr * 0.5m;

        var yesterdayLogsCalories = await _context.FoodLogs
            .Where(l => l.UserId == userId && l.LogDate == yesterday)
            .SumAsync(l => l.CaloriesKcal);

        if (limit > 0 && yesterdayLogsCalories >= limit)
        {
            throw new BusinessException("STREAK_ALREADY_COMPLETED", "Yesterday already has sufficient logs, no freeze needed.");
        }

        // Add freeze transaction
        var transaction = new StreakFreezeTransaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProtectedDate = yesterday,
            Source = 2, // Manual
            CreatedAt = DateTime.UtcNow
        };

        _context.StreakFreezeTransactions.Add(transaction);
        streak.FreezeCount -= 1;

        await _context.SaveChangesAsync();
        return true;
    }
}
