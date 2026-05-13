using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Models.Subscriptions;

public class Subscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int PlanId { get; set; }
    
    /// <summary>
    /// 0 = Active, 1 = Trialing, 2 = Cancelled, 3 = Expired
    /// </summary>
    public int Status { get; set; }
    
    public DateTime CurrentPeriodEnd { get; set; }
    public string? StoreTransactionId { get; set; } // Apple/Google transaction ID
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public SubscriptionPlan Plan { get; set; } = null!;
    public ICollection<SubscriptionEvent> Events { get; set; } = new List<SubscriptionEvent>();
}
