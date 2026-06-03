using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Notifications;

public class RegisterDeviceTokenRequest
{
    [Required]
    public string Token { get; set; } = null!;

    public string? DeviceType { get; set; } // "ios", "android", "web", "expo"
}
