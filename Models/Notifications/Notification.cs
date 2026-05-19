using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Models.Notifications;

/// <summary>
/// Lịch sử thông báo đã gửi cho người dùng
/// </summary>
public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int NotificationTypeId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Dữ liệu bổ sung (JSON format)
    /// Ví dụ: {"mealTypeId": 1, "targetScreen": "FoodLog"}
    /// </summary>
    public string? Data { get; set; }
    
    /// <summary>
    /// Đã đọc chưa
    /// </summary>
    public bool IsRead { get; set; } = false;
    
    /// <summary>
    /// Thời gian đọc
    /// </summary>
    public DateTime? ReadAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    public NotificationType NotificationType { get; set; } = null!;
}
