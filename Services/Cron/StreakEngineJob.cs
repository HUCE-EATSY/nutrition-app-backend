using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using nutrition_app_backend.Data;
using nutrition_app_backend.Models.Users;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace nutrition_app_backend.Services.Cron
{
    public class StreakEngineJob : BackgroundService
    {
        private readonly ILogger<StreakEngineJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public StreakEngineJob(ILogger<StreakEngineJob> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StreakEngineJob started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                // Schedule to run at next midnight UTC
                var nextRun = now.Date.AddDays(1);
                var delay = nextRun - now;

                _logger.LogInformation("Next streak processing at {time}", nextRun);

                // Wait until next run
                await Task.Delay(delay, stoppingToken);

                if (stoppingToken.IsCancellationRequested)
                    break;

                _logger.LogInformation("Processing daily streaks at {time}", DateTime.UtcNow);

                try
                {
                    await ProcessStreaksAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing streaks");
                }
            }
        }

        private async Task ProcessStreaksAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WaoDbContext>();

            var yesterday = DateTime.UtcNow.Date.AddDays(-1);
            var yesterdayOnly = DateOnly.FromDateTime(yesterday);

            var streaks = await context.UserStreaks.Include(s => s.User).ThenInclude(u => u.Goals).ToListAsync(stoppingToken);

            foreach (var streak in streaks)
            {
                if (streak.LastLogDate.HasValue && streak.LastLogDate.Value.Date == DateTime.UtcNow.Date)
                {
                    continue; 
                }

                var activeGoal = streak.User.Goals.OrderByDescending(g => g.Id).FirstOrDefault();
                decimal bmr = activeGoal?.BmrKcal ?? 1500m;
                decimal targetKcal = bmr * 0.5m;

                var totalCals = await context.FoodLogs
                    .Where(f => f.UserId == streak.UserId && f.LogDate == yesterdayOnly)
                    .SumAsync(f => f.CaloriesKcal, stoppingToken);

                if (totalCals >= targetKcal)
                {
                    streak.CurrentStreak += 1;
                    if (streak.CurrentStreak > streak.LongestStreak)
                    {
                        streak.LongestStreak = streak.CurrentStreak;
                    }
                    streak.LastLogDate = DateTime.UtcNow;
                }
                else
                {
                    var alreadyFrozen = await context.StreakFreezeTransactions
                        .AnyAsync(f => f.UserId == streak.UserId && f.FreezeDate.Date == yesterday, stoppingToken);

                    if (!alreadyFrozen && streak.FreezeCount > 0)
                    {
                        streak.FreezeCount -= 1;
                        var trans = new StreakFreezeTransaction
                        {
                            UserId = streak.UserId,
                            FreezeDate = yesterday,
                            Source = 1 // Auto
                        };
                        context.StreakFreezeTransactions.Add(trans);
                    }
                    else if (!alreadyFrozen)
                    {
                        // Reset
                        streak.CurrentStreak = 0;
                    }
                }
            }

            await context.SaveChangesAsync(stoppingToken);
        }
    }
}
