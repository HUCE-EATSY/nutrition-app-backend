using AutoMapper;
using nutrition_app_backend.Data;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.DTOs.Users;
using nutrition_app_backend.Enums;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Models.Users;
using nutrition_app_backend.Services.Storage;

namespace nutrition_app_backend.Services.User;

public class UserService : IUserService
{
    private readonly WaoDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IStorageService _storage;

    public UserService(WaoDbContext dbContext, IMapper mapper, IStorageService storage)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _storage = storage;
    }

    public async Task<UserGoalResponse> OnboardUserAsync(Guid userId, OnboardingRequest request)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found.");
        
        var existingProfile = await _dbContext.UserProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (existingProfile != null)
        {
            existingProfile.DisplayName = request.DisplayName;
            existingProfile.Gender = request.Gender;
            existingProfile.DateOfBirth = request.DateOfBirth;
            existingProfile.HeightCm = request.HeightCm;
            existingProfile.WeightKg = request.WeightKg;
            existingProfile.UpdatedAt = DateTime.UtcNow;

            var existingGoals = await _dbContext.UserGoals
                .Where(x => x.UserId == userId && x.IsActive)
                .ToListAsync();
            foreach (var eg in existingGoals)
            {
                eg.IsActive = false;
            }
        }
        else
        {
            var profile = new UserProfile
            {
                UserId = userId,
                DisplayName = request.DisplayName,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                HeightCm = request.HeightCm,
                WeightKg = request.WeightKg,
            };
            _dbContext.UserProfiles.Add(profile);
        }

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
                throw new BusinessException("INVALID_GENDER", "Invalid gender for BMR calculation.");
        } 

        decimal[] activityMultipliers = { 0, 1.2m, 1.375m, 1.55m, 1.725m, 1.9m };
        decimal tdee = bmr * activityMultipliers[request.ActivityLevel];

        // Calculate dynamic target calories based on goal type: 1 = Lose (-500), 2 = Gain (+500), 3 = Maintain (+0)
        decimal targetCalories = request.GoalType switch
        {
            1 => tdee - 500,
            2 => tdee + 500,
            _ => tdee
        };
        if (targetCalories < 1200) targetCalories = 1200; // Safe minimum boundary

        // Calculate target completion date dynamically using request.WeeklyGoalKg
        decimal weightDiff = Math.Abs(request.WeightKg - request.GoalWeightKg);
        int weeksNeeded = 0;
        if (request.GoalType != 3) // 1 = Lose, 2 = Gain, 3 = Maintain
        {
            decimal weeklyGoal = request.WeeklyGoalKg > 0 ? request.WeeklyGoalKg : 0.5m;
            weeksNeeded = (int)Math.Ceiling(weightDiff / weeklyGoal);
        }
        DateTime targetDate = DateTime.UtcNow.AddDays(weeksNeeded * 7);

        // 3. KHỞI TẠO GOAL
        var goal = new UserGoal
        {
            UserId = userId,
            WeightKg = request.WeightKg,
            GoalWeightKg = request.GoalWeightKg,
            WeeklyGoalKg = request.WeeklyGoalKg,
            ActivityLevel = request.ActivityLevel,
            GoalType = request.GoalType,
            BmrKcal = bmr,
            TdeeKcal = tdee,
            TargetCalories = targetCalories,
            TargetProteinG = targetCalories * 0.3m / 4,
            TargetCarbsG = targetCalories * 0.4m / 4,
            TargetFatG = targetCalories * 0.3m / 9,
            TargetDate = targetDate,
            IsActive = true
        };
        _dbContext.UserGoals.Add(goal);

        await _dbContext.SaveChangesAsync();
        
        return _mapper.Map<UserGoalResponse>(goal);
    }

    public async Task<UserProfileResponse> UpdateUserProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found.");

        var profile = await _dbContext.UserProfiles.FindAsync(userId);
        if (profile == null)
            throw new NotFoundException("User profile not found.");

        // 1. CẬP NHẬT PROFILE
        profile.DisplayName = request.DisplayName;
        profile.AvatarUrl = request.AvatarUrl;
        profile.Gender = request.Gender;
        profile.DateOfBirth = request.DateOfBirth;
        profile.HeightCm = request.HeightCm;
        profile.WeightKg = request.WeightKg;
        profile.UpdatedAt = DateTime.UtcNow;

        // 2. CẬP NHẬT GOAL NẾU CÓ THAY ĐỔI
        var goal = await _dbContext.UserGoals.FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);
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
                    throw new BusinessException("INVALID_GENDER", "Invalid gender for BMR calculation.");
            }

            decimal[] activityMultipliers = { 0, 1.2m, 1.375m, 1.55m, 1.725m, 1.9m };
            decimal tdee = bmr * activityMultipliers[request.ActivityLevel];

            // Calculate dynamic target calories based on goal type stored in DB
            decimal targetCalories = goal.GoalType switch
            {
                1 => tdee - 500,
                2 => tdee + 500,
                _ => tdee
            };
            if (targetCalories < 1200) targetCalories = 1200; // Safe minimum boundary

            // Cập nhật goal
            goal.WeightKg = request.WeightKg;
            goal.ActivityLevel = request.ActivityLevel;
            goal.BmrKcal = bmr;
            goal.TdeeKcal = tdee;
            goal.TargetCalories = targetCalories;
            goal.TargetProteinG = targetCalories * 0.3m / 4;
            goal.TargetCarbsG = targetCalories * 0.4m / 4;
            goal.TargetFatG = targetCalories * 0.3m / 9;

            // Recalculate TargetDate
            decimal weightDiff = Math.Abs(request.WeightKg - (goal.GoalWeightKg ?? request.WeightKg));
            int weeksNeeded = 0;
            if (goal.GoalType != 3)
            {
                decimal weeklyGoal = goal.WeeklyGoalKg > 0 ? goal.WeeklyGoalKg : 0.5m;
                weeksNeeded = (int)Math.Ceiling(weightDiff / weeklyGoal);
            }
            goal.TargetDate = DateTime.UtcNow.AddDays(weeksNeeded * 7);
        }

        await _dbContext.SaveChangesAsync();
        
        return _mapper.Map<UserProfileResponse>(profile);
    }

    public async Task<UserGoalUpdateResponse> UpdateUserGoalAsync(Guid userId, UpdateUserGoalRequest request)
    {
        var goal = await _dbContext.UserGoals.FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);
        if (goal == null)
            throw new NotFoundException("User goal not found.");

        // Cập nhật goal
        goal.GoalType = request.GoalType;
        goal.GoalWeightKg = request.GoalWeightKg;
        goal.WeeklyGoalKg = request.WeeklyGoalKg;
        goal.TargetCalories = request.TargetCalories;
        goal.TargetProteinG = request.TargetProteinG;
        goal.TargetCarbsG = request.TargetCarbsG;
        goal.TargetFatG = request.TargetFatG;

        // Recalculate TargetDate
        decimal weightDiff = Math.Abs(goal.WeightKg - (request.GoalWeightKg ?? goal.WeightKg));
        int weeksNeeded = 0;
        if (request.GoalType != 3)
        {
            decimal weeklyGoal = request.WeeklyGoalKg > 0 ? request.WeeklyGoalKg : 0.5m;
            weeksNeeded = (int)Math.Ceiling(weightDiff / weeklyGoal);
        }
        goal.TargetDate = DateTime.UtcNow.AddDays(weeksNeeded * 7);

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<UserGoalUpdateResponse>(goal);
    }

    public async Task<GetUserInfoResponse> GetUserInfoAsync(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found.");

        var profile = await _dbContext.UserProfiles.FindAsync(userId);
        var activeGoal = await _dbContext.UserGoals.FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);

        return new GetUserInfoResponse
        {
            UserId = user.Id,
            Profile = profile != null ? _mapper.Map<UserProfileResponse>(profile) : null,
            ActiveGoal = activeGoal != null ? _mapper.Map<UserGoalResponse>(activeGoal) : null,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    /// <summary>
    /// Upload avatar lên Cloudinary, cập nhật AvatarUrl trong profile.
    /// Trả về avatar_url mới để frontend hiển thị ngay.
    /// </summary>
    public async Task<string> UploadAvatarAsync(Guid userId, IFormFile file)
    {
        var profile = await _dbContext.UserProfiles.FindAsync(userId);
        if (profile == null)
            throw new NotFoundException("User profile not found. Please complete onboarding first.");

        // Validate file
        if (file == null || file.Length == 0)
            throw new BusinessException("INVALID_FILE", "Vui lòng chọn ảnh hợp lệ.");

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            throw new BusinessException("INVALID_FILE_TYPE", "Chỉ chấp nhận ảnh JPEG, PNG hoặc WebP.");

        if (file.Length > 5 * 1024 * 1024)
            throw new BusinessException("FILE_TOO_LARGE", "Ảnh không được vượt quá 5MB.");

        // Upload lên Cloudinary, folder riêng cho avatar
        var publicId = await _storage.UploadAsync(file, folder: "wao/avatars");
        var avatarUrl = _storage.BuildUrl(publicId);

        profile.AvatarUrl = avatarUrl;
        profile.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return avatarUrl;
    }

    /// <summary>
    /// Xóa tài khoản user (soft delete).
    /// - Đánh dấu DeletedAt trên bản ghi User.
    /// - Thu hồi toàn bộ RefreshToken còn hiệu lực để ngăn đăng nhập lại.
    /// </summary>
    public async Task DeleteAccountAsync(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found.");

        if (user.DeletedAt.HasValue)
            throw new BusinessException("ACCOUNT_ALREADY_DELETED", "Tài khoản đã bị xóa trước đó.");

        var now = DateTime.UtcNow;

        // Soft delete user
        user.DeletedAt = now;
        user.UpdatedAt = now;

        // Thu hồi tất cả refresh token còn hiệu lực
        var activeTokens = _dbContext.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null);
        
        foreach (var token in activeTokens)
            token.RevokedAt = now;

        await _dbContext.SaveChangesAsync();
    }
}

