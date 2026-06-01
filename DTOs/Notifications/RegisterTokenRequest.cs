using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Notifications;

public class RegisterTokenRequest
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string Platform { get; set; }
}
