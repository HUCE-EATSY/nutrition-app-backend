using System;

namespace nutrition_app_backend.Models.Users
{
    public class UserStreak
    {
        public Guid UserId { get; set; }
        public int CurrentStreak { get; set; } = 0;
        public int LongestStreak { get; set; } = 0;
        public int FreezeCount { get; set; } = 0;
        public DateTime? LastLogDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; } = null!;
    }
}
