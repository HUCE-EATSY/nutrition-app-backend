namespace nutrition_app_backend.Models.Exercises;

/// <summary>
/// Danh mục bài tập (Cardio, Strength, Yoga, Sports, etc.)
/// </summary>
public class ExerciseCategory
{
    public int Id { get; set; }
    public string NameVi { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public int DisplayOrder { get; set; }
    
    // Navigation
    public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
}
