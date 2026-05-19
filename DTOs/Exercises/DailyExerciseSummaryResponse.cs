namespace nutrition_app_backend.DTOs.Exercises;

public class DailyExerciseSummaryResponse
{
    public DateOnly Date { get; set; }
    public int TotalDurationMinutes { get; set; }
    public decimal TotalCaloriesBurned { get; set; }
    public int ExerciseCount { get; set; }
    public List<ExerciseLogResponse> Logs { get; set; } = new();
}
