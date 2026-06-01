using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Notifications;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Models.Notifications;

namespace nutrition_app_backend.Services.Notification;

public class NotificationService : INotificationService
{
    private readonly WaoDbContext _context;

    public NotificationService(WaoDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserNotificationSettingResponse>> GetNotificationSettingsAsync(Guid userId)
    {
        // Lấy tất cả loại thông báo
        var notificationTypes = await _context.NotificationTypes.ToListAsync();

        // Lấy cài đặt hiện tại của user
        var userSettings = await _context.UserNotificationSettings
            .Where(s => s.UserId == userId)
            .Include(s => s.NotificationType)
            .ToListAsync();

        // Tạo response cho tất cả loại thông báo
        var response = notificationTypes.Select(nt =>
        {
            var setting = userSettings.FirstOrDefault(s => s.NotificationTypeId == nt.Id);
            return new UserNotificationSettingResponse
            {
                Id = setting?.Id ?? Guid.Empty,
                NotificationTypeId = nt.Id,
                NotificationTypeCode = nt.Code,
                NotificationNameVi = nt.NameVi,
                NotificationNameEn = nt.NameEn,
                IsEnabled = setting?.IsEnabled ?? false,
                ReminderTime = setting?.ReminderTime,
                DaysOfWeek = setting?.DaysOfWeek
            };
        }).ToList();

        return response;
    }

    public async Task<UserNotificationSettingResponse> UpdateNotificationSettingAsync(Guid userId, UpdateNotificationSettingRequest request)
    {
        // Kiểm tra notification type có tồn tại không
        var notificationType = await _context.NotificationTypes.FindAsync(request.NotificationTypeId);
        if (notificationType == null)
            throw new NotFoundException("Notification type not found");

        // Tìm setting hiện tại
        var setting = await _context.UserNotificationSettings
            .FirstOrDefaultAsync(s => s.UserId == userId && s.NotificationTypeId == request.NotificationTypeId);

        if (setting == null)
        {
            // Tạo mới nếu chưa có
            setting = new UserNotificationSetting
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                NotificationTypeId = request.NotificationTypeId,
                IsEnabled = request.IsEnabled,
                ReminderTime = request.ReminderTime,
                DaysOfWeek = request.DaysOfWeek,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.UserNotificationSettings.Add(setting);
        }
        else
        {
            // Cập nhật
            setting.IsEnabled = request.IsEnabled;
            setting.ReminderTime = request.ReminderTime;
            setting.DaysOfWeek = request.DaysOfWeek;
            setting.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return new UserNotificationSettingResponse
        {
            Id = setting.Id,
            NotificationTypeId = setting.NotificationTypeId,
            NotificationTypeCode = notificationType.Code,
            NotificationNameVi = notificationType.NameVi,
            NotificationNameEn = notificationType.NameEn,
            IsEnabled = setting.IsEnabled,
            ReminderTime = setting.ReminderTime,
            DaysOfWeek = setting.DaysOfWeek
        };
    }

    public async Task<List<NotificationResponse>> GetNotificationsAsync(Guid userId, bool? isRead, int page, int pageSize)
    {
        var query = _context.Notifications
            .Include(n => n.NotificationType)
            .Where(n => n.UserId == userId);

        if (isRead.HasValue)
            query = query.Where(n => n.IsRead == isRead.Value);

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificationResponse
            {
                Id = n.Id,
                NotificationTypeId = n.NotificationTypeId,
                NotificationTypeCode = n.NotificationType.Code,
                Title = n.Title,
                Message = n.Message,
                Data = n.Data,
                IsRead = n.IsRead,
                ReadAt = n.ReadAt,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync();

        return notifications;
    }

    public async Task<int> MarkAsReadAsync(Guid userId, List<Guid> notificationIds)
    {
        var notifications = await _context.Notifications
            .Where(n => notificationIds.Contains(n.Id) && n.UserId == userId && !n.IsRead)
            .ToListAsync();

        if (!notifications.Any())
            return 0;

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return notifications.Count;
    }

    public async Task<int> MarkAllAsReadAsync(Guid userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        if (!notifications.Any())
            return 0;

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return notifications.Count;
    }

    public async Task DeleteNotificationAsync(Guid userId, Guid notificationId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification == null)
            throw new NotFoundException("Notification not found");

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();
    }

    private static readonly HttpClient _httpClient = new HttpClient();

    public async Task RegisterPushTokenAsync(Guid userId, string token, string platform)
    {
        var existingToken = await _context.UserPushTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Token == token);

        if (existingToken == null)
        {
            var newToken = new UserPushToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = token,
                Platform = platform,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.UserPushTokens.Add(newToken);

            // Tự động tạo cài đặt mặc định nếu user chưa có
            var hasSettings = await _context.UserNotificationSettings.AnyAsync(s => s.UserId == userId);
            if (!hasSettings)
            {
                var types = await _context.NotificationTypes.ToListAsync();
                var now = DateTime.UtcNow;
                foreach (var type in types)
                {
                    string? defaultTime = type.Code switch
                    {
                        "MEAL_REMINDER" => "08:00",
                        "EXERCISE_REMINDER" => "17:00",
                        "WATER_REMINDER" => "10:00",
                        "DAILY_SUMMARY" => "21:00",
                        "WEEKLY_REPORT" => "09:00",
                        _ => null
                    };
                    
                    var setting = new UserNotificationSetting
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        NotificationTypeId = type.Id,
                        IsEnabled = true,
                        ReminderTime = defaultTime,
                        DaysOfWeek = type.Code == "WEEKLY_REPORT" ? "8" : "2,3,4,5,6,7,8",
                        CreatedAt = now,
                        UpdatedAt = now
                    };
                    _context.UserNotificationSettings.Add(setting);
                }
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task SendPushNotificationAsync(Guid userId, string title, string body, object? data = null)
    {
        var tokens = await _context.UserPushTokens
            .Where(t => t.UserId == userId)
            .Select(t => t.Token)
            .ToListAsync();

        if (!tokens.Any())
            return;

        var messages = tokens.Select(token => new
        {
            to = token,
            sound = "default",
            title = title,
            body = body,
            data = data
        }).ToList();

        var json = System.Text.Json.JsonSerializer.Serialize(messages);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        try
        {
            await _httpClient.PostAsync("https://exp.host/--/api/v2/push/send", content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending push notification: {ex.Message}");
        }
    }
}
