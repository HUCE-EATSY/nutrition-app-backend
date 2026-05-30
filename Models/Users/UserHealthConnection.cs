using nutrition_app_backend.Enums;

namespace nutrition_app_backend.Models.Users;

public class UserHealthConnection
{
    public Guid UserId { get; set; }
    public HealthProvider Provider { get; set; }
    public byte Status { get; set; }
    public DateTime? ConnectedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public User User { get; set; } = null!;
}
