namespace nutrition_app_backend.Models.Notifications;

/// <summary>
/// Loại thông báo
/// </summary>
public class NotificationType
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty; // MEAL_REMINDER, EXERCISE_REMINDER, WEIGHT_LOG, GOAL_ACHIEVED, etc.
    public string NameVi { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Navigation
    public ICollection<UserNotificationSetting> UserSettings { get; set; } = new List<UserNotificationSetting>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
