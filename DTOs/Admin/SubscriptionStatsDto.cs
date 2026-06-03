namespace nutrition_app_backend.DTOs.Admin;

public class SubscriptionStatsDto
{
    public int TotalPremium { get; set; }      // Tổng số user từng có Premium
    public int ActivePremium { get; set; }     // Số user đang có Premium active
    public int ExpiredPremium { get; set; }    // Số user Premium đã hết hạn
    public decimal TotalRevenue { get; set; }  // Tổng doanh thu
    public decimal MonthlyRevenue { get; set; } // Doanh thu tháng này
}
