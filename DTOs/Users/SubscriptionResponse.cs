using System;

namespace nutrition_app_backend.DTOs.Users;

public class SubscriptionResponse
{
    public string PlanId { get; set; } = null!;
    public string PlanName { get; set; } = null!;
    public string Status { get; set; } = null!; // "active", "trialing", "cancelled", "expired", "free"
    public DateTime CurrentPeriodEnd { get; set; }
}
