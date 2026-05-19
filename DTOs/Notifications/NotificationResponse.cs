namespace nutrition_app_backend.DTOs.Notifications;

public class NotificationResponse
{
    public Guid Id { get; set; }
    public int NotificationTypeId { get; set; }
    public string NotificationTypeCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Data { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
