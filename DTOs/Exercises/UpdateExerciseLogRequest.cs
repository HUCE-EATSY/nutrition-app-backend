using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Exercises;

public class UpdateExerciseLogRequest
{
    [Range(1, 1440)]
    public int? DurationMinutes { get; set; }
    
    [Range(1, 3)]
    public byte? Intensity { get; set; }
    
    public string? Notes { get; set; }
}
