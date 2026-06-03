using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using nutrition_app_backend.Data;
using nutrition_app_backend.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace nutrition_app_backend.Services.Cron
{
    public class StreakEngineJob : BackgroundService
    {
        private readonly ILogger<StreakEngineJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public StreakEngineJob(ILogger<StreakEngineJob> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StreakEngineJob started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                int intervalMinutes = _configuration.GetValue<int?>("StreakCronIntervalMinutes") ?? 0;
                TimeSpan delay;

                if (intervalMinutes > 0)
                {
                    delay = TimeSpan.FromMinutes(intervalMinutes);
                    _logger.LogInformation("Streak processing interval configured to {minutes} minutes.", intervalMinutes);
                }
                else
                {
                    DateTime now = DateTime.UtcNow;
                    // Run at 23:59 UTC
                    DateTime nextRun = now.Date.AddHours(23).AddMinutes(59);
                    if (now >= nextRun)
                    {
                        nextRun = nextRun.AddDays(1);
                    }
                    delay = nextRun - now;
                    _logger.LogInformation("Next streak processing at {time}", nextRun);
                }

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
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                WaoDbContext context = scope.ServiceProvider.GetRequiredService<WaoDbContext>();

                                TimeZoneInfo vietnamTz = TimeZoneInfo.FindSystemTimeZoneById(
                    OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh");
                DateTime todayVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTz).Date;
                DateTime yesterdayVn = todayVn.AddDays(-1);

                DateTime yesterdayStartUtc = TimeZoneInfo.ConvertTimeToUtc(yesterdayVn, vietnamTz);
                DateTime yesterdayEndUtc = TimeZoneInfo.ConvertTimeToUtc(todayVn, vietnamTz);

                List<UserStreak> streaks = await context.UserStreaks.Include(s => s.User).ThenInclude(u => u.Goals).ToListAsync(stoppingToken);

                foreach (UserStreak streak in streaks)
                {
                    if (streak.LastLogDate.HasValue && TimeZoneInfo.ConvertTimeFromUtc(streak.LastLogDate.Value, vietnamTz).Date == todayVn)
                    {
                        continue; 
                    }

                    UserGoal? activeGoal = streak.User.Goals.Where(g => g.IsActive).OrderByDescending(g => g.CreatedAt).FirstOrDefault() 
                                         ?? streak.User.Goals.OrderByDescending(g => g.CreatedAt).FirstOrDefault();
                    decimal bmr = activeGoal?.BmrKcal ?? 1500m;
                    decimal targetKcal = bmr * 0.5m;

                    decimal totalCals = await context.FoodLogs
                        .Where(f => f.UserId == streak.UserId && f.LogDate >= yesterdayStartUtc && f.LogDate < yesterdayEndUtc)
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
                        bool alreadyFrozen = await context.StreakFreezeTransactions
                            .AnyAsync(f => f.UserId == streak.UserId && f.FreezeDate.Date == yesterdayVn, stoppingToken);

                        if (!alreadyFrozen && streak.FreezeCount > 0)
                        {
                            streak.FreezeCount -= 1;
                            StreakFreezeTransaction trans = new StreakFreezeTransaction
                            {
                                UserId = streak.UserId,
                                FreezeDate = yesterdayVn,
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
}
