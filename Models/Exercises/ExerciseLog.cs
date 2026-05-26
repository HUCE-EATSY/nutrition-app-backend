using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Models.Exercises;

/// <summary>
/// Nhật ký tập luyện của người dùng
/// </summary>
public class ExerciseLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ExerciseId { get; set; }
    
    /// <summary>
    /// Ngày tập luyện
    /// </summary>
    public DateOnly LogDate { get; set; }
    
    /// <summary>
    /// Thời gian tập (phút)
    /// </summary>
    public int DurationMinutes { get; set; }
    
    /// <summary>
    /// Cường độ: 1=Nhẹ, 2=Trung bình, 3=Nặng
    /// </summary>
    public byte Intensity { get; set; } = 2;
    
    /// <summary>
    /// Calories đốt cháy (tính toán từ MET, cân nặng, thời gian)
    /// </summary>
    public decimal CaloriesBurned { get; set; }
    
    /// <summary>
    /// Ghi chú của người dùng
    /// </summary>
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    public Exercise Exercise { get; set; } = null!;
}
