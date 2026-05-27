using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Exercises;

public class UpdateExerciseLogRequest
{
    [Range(1, 600, ErrorMessage = "Duration must be between 1 and 600 minutes")]
    public int? DurationMinutes { get; set; }
    
    [Range(1, 3, ErrorMessage = "Intensity must be 1 (Light), 2 (Moderate), or 3 (Heavy)")]
    public byte? Intensity { get; set; }
    
    [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}
