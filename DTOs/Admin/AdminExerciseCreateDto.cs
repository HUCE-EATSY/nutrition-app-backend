using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Admin;

public class AdminExerciseCreateDto
{
    [Required]
    public int CategoryId { get; set; }
    
    [Required]
    public string NameVi { get; set; } = string.Empty;
    
    [Required]
    public string NameEn { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public decimal MetValue { get; set; }
    
    public string Unit { get; set; } = "minutes";
    
    public string? IconUrl { get; set; }
}
