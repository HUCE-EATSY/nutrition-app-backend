using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Models.Streaks;

public class StreakFreezeTransaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly ProtectedDate { get; set; }
    
    /// <summary>
    /// 1 = Free Plan, 2 = Premium Plan (for logging purposes)
    /// </summary>
    public int Source { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public User User { get; set; } = null!;
}
