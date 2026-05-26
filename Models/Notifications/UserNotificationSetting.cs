using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Models.Notifications;

/// <summary>
/// Cài đặt thông báo của người dùng
/// </summary>
public class UserNotificationSetting
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int NotificationTypeId { get; set; }
    
    /// <summary>
    /// Bật/tắt thông báo
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// Thời gian nhắc nhở (cho các loại thông báo có lịch)
    /// Format: HH:mm (ví dụ: "07:00", "12:00", "18:00")
    /// </summary>
    public string? ReminderTime { get; set; }
    
    /// <summary>
    /// Các ngày trong tuần (cho thông báo lặp lại)
    /// Format: "1,2,3,4,5" (2=Thứ 2, 3=Thứ 3, ..., 8=Chủ nhật)
    /// </summary>
    public string? DaysOfWeek { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    public NotificationType NotificationType { get; set; } = null!;
}
