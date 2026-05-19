namespace nutrition_app_backend.DTOs.Notifications;

public class UserNotificationSettingResponse
{
    public Guid Id { get; set; }
    public int NotificationTypeId { get; set; }
    public string NotificationTypeCode { get; set; } = string.Empty;
    public string NotificationNameVi { get; set; } = string.Empty;
    public string NotificationNameEn { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string? ReminderTime { get; set; }
    public string? DaysOfWeek { get; set; }
}
