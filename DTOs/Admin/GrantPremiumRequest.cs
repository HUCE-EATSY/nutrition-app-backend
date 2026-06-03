namespace nutrition_app_backend.DTOs.Admin;

public class GrantPremiumRequest
{
    public int PlanId { get; set; }          // 2 = Monthly, 3 = Yearly
    public int? DurationDays { get; set; }   // Tùy chỉnh (optional, mặc định theo plan)
    public string? Note { get; set; }        // Ghi chú admin
}
