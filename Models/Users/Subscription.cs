using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace nutrition_app_backend.Models.Users;

public class Subscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string PlanId { get; set; } = null!;
    
    /// <summary>
    /// 0: Active, 1: Trialing, 2: Cancelled, 3: Expired
    /// </summary>
    public int Status { get; set; }
    
    public DateTime CurrentPeriodEnd { get; set; }
    public string? StoreTransactionId { get; set; }
    
    [Column(TypeName = "timestamp(6)")]
    public DateTime CreatedAt { get; set; }
    
    [Column(TypeName = "timestamp(6)")]
    public DateTime UpdatedAt { get; set; }
    
    public User User { get; set; } = null!;
    public SubscriptionPlan Plan { get; set; } = null!;
    public ICollection<SubscriptionEvent> Events { get; set; } = new List<SubscriptionEvent>();
}
