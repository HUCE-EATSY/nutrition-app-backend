namespace nutrition_app_backend.DTOs.Admin;

public class AdminUserStatsDto
{
    public int Total { get; set; }
    public int Premium { get; set; }
    public int Free { get; set; }
    public int Locked { get; set; }
}
