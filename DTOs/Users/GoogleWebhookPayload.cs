using System;

namespace nutrition_app_backend.DTOs.Users;

public class GoogleWebhookPayload
{
    public string EventType { get; set; } = null!;
    public string SubscriptionId { get; set; } = null!;
    public string PurchaseToken { get; set; } = null!;
    public string OrderId { get; set; } = null!;
    public DateTime ExpiryTime { get; set; }
    public Guid? AppAccountToken { get; set; }
}
