namespace nutrition_app_backend.DTOs.Exercises;

public class ExerciseLogResponse
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public string ExerciseNameVi { get; set; } = string.Empty;
    public string ExerciseNameEn { get; set; } = string.Empty;
    public string? ExerciseIconUrl { get; set; }
    public DateOnly LogDate { get; set; }
    public int DurationMinutes { get; set; }
    public byte Intensity { get; set; }
    public decimal CaloriesBurned { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
