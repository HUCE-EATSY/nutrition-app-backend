using System;

namespace nutrition_app_backend.Models.Users
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // e.g. "FREE", "MONTHLY_PREMIUM", "YEARLY_PREMIUM"
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
