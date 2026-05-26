namespace nutrition_app_backend.DTOs.Exercises;

public class ExerciseCategoryResponse
{
    public int Id { get; set; }
    public string NameVi { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public List<ExerciseResponse> Exercises { get; set; } = new();
}
