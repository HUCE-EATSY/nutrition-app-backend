using System;

namespace nutrition_app_backend.Models.Users
{
    public class StreakFreezeTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public DateTime FreezeDate { get; set; }
        public byte Source { get; set; } // 1 = Auto Cron, 2 = Manual Yesterday
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; } = null!;
    }
}
