using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.Models.Notifications;
using System.Text.Json;
using System.Text;

namespace nutrition_app_backend.Services.Notification;

public class NotificationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationBackgroundService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public NotificationBackgroundService(IServiceProvider serviceProvider, ILogger<NotificationBackgroundService> logger, IHttpClientFactory httpClientFactory)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessNotificationsAsync(stoppingToken);
                // Run every 1 hour (can be adjusted)
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing notification background task.");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("Notification Background Service is stopping.");
    }

    private async Task ProcessNotificationsAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Processing notifications...");

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WaoDbContext>();
        
        var today = DateTime.UtcNow.Date;

        var usersWithTokens = await context.UserDeviceTokens
            .GroupBy(t => t.UserId)
            .Select(g => new {
                UserId = g.Key,
                Tokens = g.Select(t => t.DeviceToken).ToList()
            })
            .ToListAsync(stoppingToken);
            
        var client = _httpClientFactory.CreateClient();

        foreach (var userTokens in usersWithTokens)
        {
            var userId = userTokens.UserId;
            var tokens = userTokens.Tokens;

            // Check water reminder (Type 4)
            var waterReminderSent = await context.Notifications
                .AnyAsync(n => n.UserId == userId && n.NotificationTypeId == 4 && n.CreatedAt >= today, stoppingToken);

            if (!waterReminderSent)
            {
                await SendPushNotification(client, tokens, "Đến giờ uống nước rồiii 💧", "Hãy uống 1 ly nước để cơ thể luôn khỏe mạnh nhé!");
                context.Notifications.Add(new Models.Notifications.Notification { UserId = userId, NotificationTypeId = 4, Title = "Đến giờ uống nước rồiii 💧", Message = "Hãy uống 1 ly nước để cơ thể luôn khỏe mạnh nhé!", CreatedAt = DateTime.UtcNow });
            }

            // Check exercise reminder (Type 2)
            var exerciseReminderSent = await context.Notifications
                .AnyAsync(n => n.UserId == userId && n.NotificationTypeId == 2 && n.CreatedAt >= today, stoppingToken);

            var todayDateOnly = DateOnly.FromDateTime(today);

            if (!exerciseReminderSent)
            {
                var hasExercisedToday = await context.ExerciseLogs
                    .AnyAsync(e => e.UserId == userId && e.LogDate >= todayDateOnly, stoppingToken);
                
                if (!hasExercisedToday)
                {
                    await SendPushNotification(client, tokens, "Đừng quên tập luyện! 🏃", "Bạn chưa có hoạt động thể chất nào hôm nay. Dành 15 phút tập luyện nhé!");
                    context.Notifications.Add(new Models.Notifications.Notification { UserId = userId, NotificationTypeId = 2, Title = "Đừng quên tập luyện! 🏃", Message = "Bạn chưa có hoạt động thể chất nào hôm nay. Dành 15 phút tập luyện nhé!", CreatedAt = DateTime.UtcNow });
                }
            }

            // Check meal/calories reminder (Type 1)
            var mealReminderSent = await context.Notifications
                .AnyAsync(n => n.UserId == userId && n.NotificationTypeId == 1 && n.CreatedAt >= today, stoppingToken);
                
            if (!mealReminderSent)
            {
                var goal = await context.UserGoals.FirstOrDefaultAsync(g => g.UserId == userId, stoppingToken);
                if (goal != null && goal.TargetCalories > 0)
                {
                    var caloriesEaten = await context.FoodLogs
                        .Where(f => f.UserId == userId && f.LogDate >= todayDateOnly)
                        .SumAsync(f => f.CaloriesKcal, stoppingToken);
                        
                    if (caloriesEaten < goal.TargetCalories - 200) // still need more than 200 kcal
                    {
                        var remaining = goal.TargetCalories - caloriesEaten;
                        await SendPushNotification(client, tokens, "Ghi chép bữa ăn 🍽️", $"Bạn còn thiếu khoảng {remaining:0} kcal để đạt mục tiêu hôm nay. Đừng quên ghi lại bữa ăn nhé!");
                        context.Notifications.Add(new Models.Notifications.Notification { UserId = userId, NotificationTypeId = 1, Title = "Ghi chép bữa ăn 🍽️", Message = $"Bạn còn thiếu khoảng {remaining:0} kcal để đạt mục tiêu hôm nay. Đừng quên ghi lại bữa ăn nhé!", CreatedAt = DateTime.UtcNow });
                    }
                }
            }
        }
        
        await context.SaveChangesAsync(stoppingToken);

        _logger.LogInformation("Finished processing notifications.");
    }
    
    private async Task SendPushNotification(HttpClient client, List<string> tokens, string title, string body)
    {
        foreach (var token in tokens)
        {
            var payload = new
            {
                to = token,
                sound = "default",
                title = title,
                body = body
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            try
            {
                await client.PostAsync("https://exp.host/--/api/v2/push/send", content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send push notification to token {Token}", token);
            }
        }
    }
}
