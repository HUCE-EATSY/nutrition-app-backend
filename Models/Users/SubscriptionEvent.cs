using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace nutrition_app_backend.Models.Users;

public class SubscriptionEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SubscriptionId { get; set; }
    
    public string EventType { get; set; } = null!;
    public string RawPayload { get; set; } = null!;
    
    [Column(TypeName = "timestamp(6)")]
    public DateTime ReceivedAt { get; set; }
    
    public Subscription Subscription { get; set; } = null!;
}
