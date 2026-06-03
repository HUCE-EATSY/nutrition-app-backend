using System;

namespace nutrition_app_backend.Models.Users
{
    public class Subscription
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public int PlanId { get; set; }
        public byte Status { get; set; } // 0 = Active, 1 = Trialing, 2 = Cancelled, 3 = Expired
        public DateTime CurrentPeriodEnd { get; set; }
        public string StoreTransactionId { get; set; } = string.Empty;
        public string LatestOrderId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; } = null!;
        public SubscriptionPlan Plan { get; set; } = null!;
    }
}
