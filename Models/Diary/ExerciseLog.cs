namespace nutrition_app_backend.Models.Diary;

public class ExerciseLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string ActivityId { get; set; } = string.Empty;    // "running", "cycling"...
    public string ActivityLabel { get; set; } = string.Empty;  // "Chạy bộ"

    public string DateISO { get; set; } = string.Empty; // "YYYY-MM-DD"
    public int Hour { get; set; }

    public int DurationMinutes { get; set; }
    public decimal CaloriesBurned { get; set; }

    public DateTime LoggedAt { get; set; }
}
