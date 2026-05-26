using nutrition_app_backend.Models.Users;
using nutrition_app_backend.Enums;

namespace nutrition_app_backend.Models.Diaries;

public class StepLog
{
    public ulong Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly LogDate { get; set; }
    public int Steps { get; set; }
    public int StepGoal { get; set; }
    public HealthProvider? Provider { get; set; }
    public decimal CaloriesBurnedKcal { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;
}
