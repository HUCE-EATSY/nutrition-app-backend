using System;

namespace nutrition_app_backend.Models.Users
{
    public class SubscriptionEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? SubscriptionId { get; set; }
        public string Provider { get; set; } = string.Empty; // "apple" or "google"
        public string EventType { get; set; } = string.Empty;
        public string RawPayload { get; set; } = string.Empty;
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Subscription? Subscription { get; set; }
    }
}
