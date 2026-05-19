using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Notifications;

public class MarkAsReadRequest
{
    [Required]
    public List<Guid> NotificationIds { get; set; } = new();
}
