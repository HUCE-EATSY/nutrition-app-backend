namespace nutrition_app_backend.DTOs.Exercises;

public class ExerciseResponse
{
    public Guid Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryNameVi { get; set; } = string.Empty;
    public string NameVi { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal MetValue { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
}
