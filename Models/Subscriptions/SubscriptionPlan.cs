using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Models.Subscriptions;

public class SubscriptionPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
