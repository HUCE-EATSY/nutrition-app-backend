using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Notifications;

public class UpdateNotificationSettingRequest
{
    [Required]
    public int NotificationTypeId { get; set; }
    
    [Required]
    public bool IsEnabled { get; set; }
    
    [RegularExpression(@"^([0-1][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "ReminderTime must be in HH:mm format")]
    public string? ReminderTime { get; set; }
    
    [RegularExpression(@"^[2-8](,[2-8])*$", ErrorMessage = "DaysOfWeek must be comma-separated numbers 2-8")]
    public string? DaysOfWeek { get; set; }
}
