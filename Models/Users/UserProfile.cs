using nutrition_app_backend.Enums;

namespace nutrition_app_backend.Models.Users;

public class UserProfile
{
    public Guid UserId { get; set; }
    public Gender Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public decimal HeightCm { get; set; }
    public decimal WeightKg { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public User User { get; set; } = null!;
}