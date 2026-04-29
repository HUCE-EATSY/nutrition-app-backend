namespace nutrition_app_backend.DTOs.Users;

public class GetUserInfoResponse
{
    public Guid UserId { get; set; }
    public UserProfileResponse? Profile { get; set; }
    public UserGoalResponse? ActiveGoal { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
