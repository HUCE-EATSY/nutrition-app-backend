using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Exercises;

public class CreateExerciseLogRequest
{
    [Required(ErrorMessage = "Exercise ID is required")]
    public Guid ExerciseId { get; set; }
    
    [Required(ErrorMessage = "Log date is required")]
    public DateOnly LogDate { get; set; }
    
    [Required(ErrorMessage = "Duration is required")]
    [Range(1, 600, ErrorMessage = "Duration must be between 1 and 600 minutes")]
    public int DurationMinutes { get; set; }
    
    [Required(ErrorMessage = "Intensity is required")]
    [Range(1, 3, ErrorMessage = "Intensity must be 1 (Light), 2 (Moderate), or 3 (Heavy)")]
    public byte Intensity { get; set; } = 2;
    
    [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}
