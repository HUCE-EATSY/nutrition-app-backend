using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Models.Diaries;

public class WeightLog
{
    public ulong Id { get; set; }
    public Guid UserId { get; set; }
    public decimal WeightKg { get; set; }
    public DateOnly LogDate { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
