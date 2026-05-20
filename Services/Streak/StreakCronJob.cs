using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Services.Streak;

public class StreakCronJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StreakCronJob> _logger;

    public StreakCronJob(IServiceProvider serviceProvider, ILogger<StreakCronJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Streak Cron Job started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRunTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 0);
            
            if (now > nextRunTime)
            {
                nextRunTime = nextRunTime.AddDays(1);
            }

            var delay = nextRunTime - now;
            _logger.LogInformation("Next run scheduled at {NextRunTime} (in {Delay} hours)", nextRunTime, delay.TotalHours);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }

            try
            {
                await ProcessStreaksAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing Streak Cron Job.");
            }
        }
    }

    public async Task ProcessStreaksAsync()
    {
        _logger.LogInformation("Processing daily streaks validation...");

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WaoDbContext>();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Get all active users (exclude banned/status = 0)
        var users = await context.Users
            .Include(u => u.Streak)
            .Include(u => u.Goals)
            .Where(u => u.Status == 1)
            .ToListAsync();

        foreach (var user in users)
        {
            var streak = user.Streak;
            if (streak == null)
            {
                streak = new UserStreak
                {
                    UserId = user.Id,
                    CurrentStreak = 0,
                    LongestStreak = 0,
                    FreezeCount = 0,
                    LastLogDate = null
                };
                context.UserStreaks.Add(streak);
            }

            // Idempotency check: if last_log_date is already today, we've already processed this user today.
            if (streak.LastLogDate == today)
            {
                continue;
            }

            // Check if there is already a freeze transaction for today (idempotency check for freeze)
            var alreadyFrozen = await context.StreakFreezeTransactions
                .AnyAsync(t => t.UserId == user.Id && t.ProtectedDate == today);
            if (alreadyFrozen)
            {
                continue;
            }

            // Fetch active goal BMR
            var activeGoal = user.Goals.FirstOrDefault(g => g.IsActive);
            decimal bmr = activeGoal?.BmrKcal ?? 0;
            decimal limit = bmr * 0.5m;

            // Fetch user's logs for today
            var todayLogsCalories = await context.FoodLogs
                .Where(l => l.UserId == user.Id && l.LogDate == today)
                .SumAsync(l => l.CaloriesKcal);

            bool meetsThreshold = limit > 0 && todayLogsCalories >= limit;

            if (meetsThreshold)
            {
                streak.LastLogDate = today;
                streak.CurrentStreak += 1;
                if (streak.CurrentStreak > streak.LongestStreak)
                {
                    streak.LongestStreak = streak.CurrentStreak;
                }
            }
            else
            {
                if (streak.FreezeCount > 0)
                {
                    var transaction = new StreakFreezeTransaction
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        ProtectedDate = today,
                        Source = 1, // System Cron
                        CreatedAt = DateTime.UtcNow
                    };
                    context.StreakFreezeTransactions.Add(transaction);

                    streak.FreezeCount -= 1;
                }
                else
                {
                    streak.CurrentStreak = 0;
                }
            }
        }

        await context.SaveChangesAsync();
        _logger.LogInformation("Daily streaks validation completed.");
    }
}
