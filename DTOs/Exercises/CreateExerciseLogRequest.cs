using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Exercises;

public class CreateExerciseLogRequest
{
    [Required]
    public Guid ExerciseId { get; set; }
    
    [Required]
    public DateOnly LogDate { get; set; }
    
    [Required]
    [Range(1, 1440)]
    public int DurationMinutes { get; set; }
    
    [Range(1, 3)]
    public byte Intensity { get; set; } = 2;
    
    public string? Notes { get; set; }
}
