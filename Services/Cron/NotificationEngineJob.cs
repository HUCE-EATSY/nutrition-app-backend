using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using nutrition_app_backend.Data;
using nutrition_app_backend.Services.Notification;
using nutrition_app_backend.Models.Notifications;

namespace nutrition_app_backend.Services.Cron;

public class NotificationEngineJob : BackgroundService
{
    private readonly ILogger<NotificationEngineJob> _logger;
    private readonly IServiceProvider _serviceProvider;

    public NotificationEngineJob(ILogger<NotificationEngineJob> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NotificationEngineJob started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            // Run exactly at the top of the minute
            var nextMinute = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddMinutes(1);
            var delay = nextMinute - now;
            await Task.Delay(delay, stoppingToken);

            try
            {
                await ProcessNotificationsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing NotificationEngineJob.");
            }
        }
    }

    private async Task ProcessNotificationsAsync(CancellationToken stoppingToken)
    {
        var currentTimeStr = DateTime.Now.ToString("HH:mm");
        _logger.LogInformation($"NotificationEngineJob checking notifications for time: {currentTimeStr}");

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WaoDbContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var settings = await context.UserNotificationSettings
            .Include(s => s.NotificationType)
            .Include(s => s.User)
            .Where(s => s.IsEnabled && s.ReminderTime == currentTimeStr)
            .ToListAsync(stoppingToken);

        if (!settings.Any())
            return;

        foreach (var setting in settings)
        {
            try
            {
                await ProcessSettingAsync(setting, context, notificationService, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing setting {setting.Id} for user {setting.UserId}");
            }
        }
    }

    private async Task ProcessSettingAsync(UserNotificationSetting setting, WaoDbContext context, INotificationService notificationService, CancellationToken stoppingToken)
    {
        var userId = setting.UserId;
        var code = setting.NotificationType.Code;
        var title = setting.NotificationType.NameVi;
        var body = "";

        // Dynamic logic per notification type
        if (code == "MEAL_REMINDER")
        {
            body = "Đã đến giờ ăn, đừng quên ghi lại bữa ăn của bạn nhé!";
        }
        else if (code == "WATER_REMINDER")
        {
            body = "Hãy nhớ uống đủ nước để cơ thể luôn khỏe mạnh!";
        }
        else if (code == "EXERCISE_REMINDER")
        {
            body = "Đã đến lúc vận động! Hãy dành thời gian cho bài tập hôm nay.";
        }
        else if (code == "GOAL_ACHIEVED")
        {
            // For Streak reminders (or goal achieved), check if they completed
            var streak = await context.UserStreaks.FirstOrDefaultAsync(s => s.UserId == userId, stoppingToken);
            if (streak != null && streak.CurrentStreak > 0)
            {
                // If they have already completed for today (assumed based on some last log date)
                // We'll just send a congratulatory message
                body = $"Tuyệt vời, bạn đã duy trì chuỗi {streak.CurrentStreak} ngày liên tiếp!";
            }
            else
            {
                body = "Hãy tiếp tục cố gắng để đạt mục tiêu hôm nay nhé!";
            }
        }
        else
        {
            body = $"Bạn có thông báo mới từ {title}";
        }

        // Send push notification
        await notificationService.SendPushNotificationAsync(userId, title, body);

        // Also save to database
        var notification = new nutrition_app_backend.Models.Notifications.Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            NotificationTypeId = setting.NotificationTypeId,
            Title = title,
            Message = body,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };
        context.Notifications.Add(notification);
        await context.SaveChangesAsync(stoppingToken);
    }
}
