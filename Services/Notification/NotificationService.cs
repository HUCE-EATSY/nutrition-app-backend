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
}
