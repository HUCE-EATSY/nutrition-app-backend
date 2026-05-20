using System.Collections.Generic;

namespace nutrition_app_backend.Models.Users;

public class SubscriptionPlan
{
    public string Id { get; set; } = null!; // e.g., "premium_monthly"
    public string Name { get; set; } = null!;
    public int DurationDays { get; set; }
    public decimal Price { get; set; }
    
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
