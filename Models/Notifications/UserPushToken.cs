using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Models.Notifications;

public class UserPushToken
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Token { get; set; }

    [MaxLength(50)]
    public string Platform { get; set; } // "ios", "android", "web"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}
