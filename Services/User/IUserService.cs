using nutrition_app_backend.DTOs.Users;

namespace nutrition_app_backend.Services.User;

public interface IUserService
{
    Task<UserGoalResponse> OnboardUserAsync(Guid userId, OnboardingRequest request);
    Task<UserProfileResponse> UpdateUserProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<UserGoalUpdateResponse> UpdateUserGoalAsync(Guid userId, UpdateUserGoalRequest request);
    Task<GetUserInfoResponse> GetUserInfoAsync(Guid userId);
}
