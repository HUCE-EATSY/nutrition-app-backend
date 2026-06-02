namespace nutrition_app_backend.DTOs.Admin;

public class AdminDashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int NewUsers7Days { get; set; }
    public int TotalFoods { get; set; }
    public int TotalExercises { get; set; }
    public int ActiveVipUsers { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal RevenueLastMonth { get; set; }
}
