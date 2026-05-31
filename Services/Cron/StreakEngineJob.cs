using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using nutrition_app_backend.Data;
using nutrition_app_backend.Services.Streak;
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

        // Vietnam timezone (UTC+7)
        private static readonly TimeZoneInfo VietnamTz = TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh");

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
                // Calculate next 23:59 in Vietnam time
                var nowUtc = DateTime.UtcNow;
                var nowVn = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, VietnamTz);

                // Target: 23:59:00 today (VN time)
                var targetVn = nowVn.Date.AddHours(23).AddMinutes(59);

                // If we're already past 23:59 today, schedule for tomorrow
                if (nowVn >= targetVn)
                    targetVn = targetVn.AddDays(1);

                // Convert target back to UTC for delay calculation
                var targetUtc = TimeZoneInfo.ConvertTimeToUtc(targetVn, VietnamTz);
                var delay = targetUtc - nowUtc;

                _logger.LogInformation("Next streak processing at {vnTime} (VN) / {utcTime} (UTC)", targetVn, targetUtc);

                await Task.Delay(delay, stoppingToken);

                if (stoppingToken.IsCancellationRequested)
                    break;

                _logger.LogInformation("Processing daily streaks at {vnTime} (VN)", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTz));

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
            var streakService = scope.ServiceProvider.GetRequiredService<IStreakService>();
            
            await streakService.ProcessStreaksAsync(stoppingToken);
        }
    }
}
