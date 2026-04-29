using AutoMapper;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Users;
using nutrition_app_backend.Enums;
using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Services.User;

public class UserService : IUserService
{
    private readonly WaoDbContext _dbContext;
    private readonly IMapper _mapper;
    
    public UserService(WaoDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<UserGoalResponse?> OnboardUserAsync(Guid userId, OnboardingRequest request)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null) return null;
        
        var existingProfile = _dbContext.UserProfiles
            .FirstOrDefault(x => x.UserId == userId);

        if (existingProfile != null)
        {
            throw new Exception("User already onboarded");
        }
        
        var profile = new UserProfile
        {
            UserId = userId,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            HeightCm = request.HeightCm,
            WeightKg = request.WeightKg,
        };
        _dbContext.UserProfiles.Add(profile);

        // 2. TÍNH TOÁN NGHIỆP VỤ 
        int age = DateTime.Now.Year - request.DateOfBirth.Year;
        decimal bmr = (10m * request.WeightKg) + (6.25m * request.HeightCm) - (5m * age);
        switch (request.Gender)
        {
            case Gender.Male:
                bmr += 5;
                break;
            case Gender.Female:
                bmr -= 161;
                break;
            default:
                throw new ArgumentException("Invalid gender for BMR calculation");
        } 

        decimal[] activityMultipliers = { 0, 1.2m, 1.375m, 1.55m, 1.725m, 1.9m };
        decimal tdee = bmr * activityMultipliers[request.ActivityLevel];

        // 3. KHỞI TẠO GOAL
        var goal = new UserGoal
        {
            UserId = userId,
            WeightKg = request.WeightKg,
            GoalWeightKg = request.GoalWeightKg,
            ActivityLevel = request.ActivityLevel,
            BmrKcal = bmr,
            TdeeKcal = tdee,
            TargetCalories = tdee - 500,
            TargetProteinG = (tdee - 500) * 0.3m / 4,
            TargetCarbsG = (tdee - 500) * 0.4m / 4,
            TargetFatG = (tdee - 500) * 0.3m / 9,
            IsActive = true
        };
        _dbContext.UserGoals.Add(goal);

        await _dbContext.SaveChangesAsync();
        
        return _mapper.Map<UserGoalResponse>(goal);
    }

    public async Task<UserProfileResponse?> UpdateUserProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null) return null;

        var profile = await _dbContext.UserProfiles.FindAsync(userId);
        if (profile == null) return null;

        // 1. CẬP NHẬT PROFILE
        profile.Gender = request.Gender;
        profile.DateOfBirth = request.DateOfBirth;
        profile.HeightCm = request.HeightCm;
        profile.WeightKg = request.WeightKg;
        profile.UpdatedAt = DateTime.UtcNow;

        // 2. CẬP NHẬT GOAL NẾU CÓ THAY ĐỔI
        var goal = _dbContext.UserGoals.FirstOrDefault(x => x.UserId == userId && x.IsActive);
        if (goal != null)
        {
            int age = DateTime.Now.Year - request.DateOfBirth.Year;
            decimal bmr = (10m * request.WeightKg) + (6.25m * request.HeightCm) - (5m * age);
            switch (request.Gender)
            {
                case Gender.Male:
                    bmr += 5;
                    break;
                case Gender.Female:
                    bmr -= 161;
                    break;
                default:
                    throw new ArgumentException("Invalid gender for BMR calculation");
            }

            decimal[] activityMultipliers = { 0, 1.2m, 1.375m, 1.55m, 1.725m, 1.9m };
            decimal tdee = bmr * activityMultipliers[request.ActivityLevel];

            // Cập nhật goal
            goal.WeightKg = request.WeightKg;
            goal.ActivityLevel = request.ActivityLevel;
            goal.BmrKcal = bmr;
            goal.TdeeKcal = tdee;
            goal.TargetCalories = tdee - 500;
            goal.TargetProteinG = (tdee - 500) * 0.3m / 4;
            goal.TargetCarbsG = (tdee - 500) * 0.4m / 4;
            goal.TargetFatG = (tdee - 500) * 0.3m / 9;
        }

        await _dbContext.SaveChangesAsync();
        
        return _mapper.Map<UserProfileResponse>(profile);
    }

    public async Task<UserGoalUpdateResponse?> UpdateUserGoalAsync(Guid userId, UpdateUserGoalRequest request)
    {
        var goal = _dbContext.UserGoals.FirstOrDefault(x => x.UserId == userId && x.IsActive);
        if (goal == null) return null;

        // Cập nhật goal
        goal.GoalType = request.GoalType;
        goal.GoalWeightKg = request.GoalWeightKg;
        goal.TargetCalories = request.TargetCalories;
        goal.TargetProteinG = request.TargetProteinG;
        goal.TargetCarbsG = request.TargetCarbsG;
        goal.TargetFatG = request.TargetFatG;

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<UserGoalUpdateResponse>(goal);
    }

    public async Task<GetUserInfoResponse?> GetUserInfoAsync(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null) return null;

        var profile = await _dbContext.UserProfiles.FindAsync(userId);
        var activeGoal = _dbContext.UserGoals.FirstOrDefault(x => x.UserId == userId && x.IsActive);

        return new GetUserInfoResponse
        {
            UserId = user.Id,
            Profile = profile != null ? _mapper.Map<UserProfileResponse>(profile) : null,
            ActiveGoal = activeGoal != null ? _mapper.Map<UserGoalResponse>(activeGoal) : null,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}