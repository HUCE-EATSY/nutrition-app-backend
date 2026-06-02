using nutrition_app_backend.DTOs.Notifications;

namespace nutrition_app_backend.Services.Notification;

public interface INotificationService
{
    Task<List<UserNotificationSettingResponse>> GetNotificationSettingsAsync(Guid userId);
    Task<UserNotificationSettingResponse> UpdateNotificationSettingAsync(Guid userId, UpdateNotificationSettingRequest request);
    Task<List<NotificationResponse>> GetNotificationsAsync(Guid userId, bool? isRead, int page, int pageSize);
    Task<int> MarkAsReadAsync(Guid userId, List<Guid> notificationIds);
    Task<int> MarkAllAsReadAsync(Guid userId);
    Task DeleteNotificationAsync(Guid userId, Guid notificationId);
    Task<int> GetUnreadCountAsync(Guid userId);
    
    // Push Notifications
    Task RegisterPushTokenAsync(Guid userId, string token, string platform);
    Task SendPushNotificationAsync(Guid userId, string title, string body, object? data = null);
}
