namespace nutrition_app_backend.Models.Subscriptions;

public class SubscriptionEvent
{
    public Guid Id { get; set; }
    public Guid SubscriptionId { get; set; }
    
    /// <summary>
    /// e.g. "trial_started", "renewed", "cancelled", "expired"
    /// </summary>
    public string EventType { get; set; } = string.Empty;
    
    /// <summary>
    /// Apple/Google raw JSON payload
    /// </summary>
    public string RawPayload { get; set; } = string.Empty;
    
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Subscription Subscription { get; set; } = null!;
}
