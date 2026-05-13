using Hangfire;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.Models.Streaks;

namespace nutrition_app_backend.Services.BackgroundTasks;

public class StreakCronJob
{
    private readonly WaoDbContext _context;

    public StreakCronJob(WaoDbContext context)
    {
        _context = context;
    }

    // This method will be scheduled by Hangfire
    public async Task ProcessDailyStreaks()
    {
        var todayDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var yesterday = todayDate.AddDays(-1);

        // Simplified logic as per requirements:
        // For each active user, check if food_logs total calories > 50% BMR
        // If yes, increase streak. If no, check for freeze. If no freeze, reset.
        
        var activeUsers = await _context.Users
            .Include(u => u.Goals.Where(g => g.IsActive))
            .Where(u => u.Status == 1)
            .ToListAsync();

        foreach (var user in activeUsers)
        {
            var goal = user.Goals.FirstOrDefault();
            if (goal == null) continue;

            var requiredCalories = goal.BmrKcal * 0.5m;

            // Note: FoodLogs relation in User needs to be properly queried here
            // Assuming we query FoodLogs for the specific date
            var consumedCalories = await _context.Set<nutrition_app_backend.Models.Diaries.FoodLog>()
                .Where(log => log.UserId == user.Id && log.LogDate == todayDate)
                .SumAsync(log => log.CaloriesKcal);

            var streak = await _context.Set<UserStreak>().FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (streak == null)
            {
                streak = new UserStreak { UserId = user.Id, CurrentStreak = 0, LongestStreak = 0, FreezeCount = 0 };
                _context.Set<UserStreak>().Add(streak);
            }

            if (consumedCalories >= requiredCalories)
            {
                streak.CurrentStreak++;
                streak.LastLogDate = todayDate;
                if (streak.CurrentStreak > streak.LongestStreak)
                {
                    streak.LongestStreak = streak.CurrentStreak;
                }
            }
            else
            {
                // Check freeze for yesterday or today depending on logic.
                // Assuming freeze was used for the missed day
                var hasFreezeTransaction = await _context.Set<StreakFreezeTransaction>()
                    .AnyAsync(f => f.UserId == user.Id && f.ProtectedDate == todayDate);

                if (hasFreezeTransaction)
                {
                    // Already frozen manually, do not reset streak
                }
                else if (streak.FreezeCount > 0)
                {
                    // Auto freeze
                    streak.FreezeCount--;
                    _context.Set<StreakFreezeTransaction>().Add(new StreakFreezeTransaction
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        ProtectedDate = todayDate,
                        Source = 1 // or derived from plan
                    });
                }
                else
                {
                    streak.CurrentStreak = 0;
                }
            }
        }

        await _context.SaveChangesAsync();
    }
}
