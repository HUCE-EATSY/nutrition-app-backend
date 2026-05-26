namespace nutrition_app_backend.Models.Exercises;

/// <summary>
/// Bài tập cụ thể (Chạy bộ, Đạp xe, Yoga, Bơi lội, etc.)
/// </summary>
public class Exercise
{
    public Guid Id { get; set; }
    public int CategoryId { get; set; }
    public string NameVi { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    /// <summary>
    /// MET (Metabolic Equivalent of Task) - Hệ số chuyển hóa năng lượng
    /// Dùng để tính calories: Calories = MET × Weight(kg) × Duration(hours)
    /// </summary>
    public decimal MetValue { get; set; }
    
    /// <summary>
    /// Đơn vị đo: minutes, km, reps, sets, etc.
    /// </summary>
    public string Unit { get; set; } = "minutes";
    
    public string? IconUrl { get; set; }
    public byte Status { get; set; } = 1; // 1: Active, 0: Inactive
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public ExerciseCategory Category { get; set; } = null!;
    public ICollection<ExerciseLog> ExerciseLogs { get; set; } = new List<ExerciseLog>();
}
