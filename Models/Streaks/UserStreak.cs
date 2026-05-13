using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Models.Streaks;

public class UserStreak
{
    public Guid UserId { get; set; }
    public int CurrentStreak { get; set; } = 0;
    public int LongestStreak { get; set; } = 0;
    public int FreezeCount { get; set; } = 0;
    public DateOnly? LastLogDate { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}
