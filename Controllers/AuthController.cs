using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Auth;
using nutrition_app_backend.Services.Auth;
using nutrition_app_backend.Services.Token;
using System;
using System.Threading.Tasks;
using RefreshRequest = nutrition_app_backend.DTOs.Auth.RefreshRequest;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly WaoDbContext _context;

    public AuthController(
        IAuthService authService,
        ITokenService tokenService,
        WaoDbContext context)
    {
        _authService = authService;
        _tokenService = tokenService;
        _context = context;
    }

    [HttpPost("google")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> LoginWithGoogle([FromBody] GoogleLoginRequest request)
    {
        var result = await _authService.LoginWithGoogleAsync(request);

        return Ok(ApiResponse<AuthResponse>.Success(result, "Đăng nhập thành công"));
    }

    [HttpPost("guest")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> LoginAsGuest()
    {
        var userId = Guid.Parse("08debd8b-4703-4879-8ad5-2d40811a86b2");
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user == null)
        {
            user = new Models.Users.User
            {
                Id = userId,
                Role = 1,
                Status = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
        }

        // Initialize Profile and Goal if they don't exist so the app doesn't crash
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null)
        {
            profile = new Models.Users.UserProfile
            {
                UserId = userId,
                DisplayName = "Khách Dùng Thử",
                Gender = Enums.Gender.Male,
                DateOfBirth = new DateOnly(1990, 1, 1),
                HeightCm = 170,
                WeightKg = 70,
                UpdatedAt = DateTime.UtcNow
            };
            _context.UserProfiles.Add(profile);
        }

        var goal = await _context.UserGoals.FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
        if (goal == null)
        {
            goal = new Models.Users.UserGoal
            {
                UserId = userId,
                GoalType = 3, // Maintain
                ActivityLevel = 2,
                WeightKg = 70,
                GoalWeightKg = 70,
                WeeklyGoalKg = 0.5m,
                BmrKcal = 1600,
                TdeeKcal = 2200,
                TargetCalories = 2200,
                TargetProteinG = 165,
                TargetCarbsG = 220,
                TargetFatG = 73,
                TargetDate = DateTime.UtcNow.AddMonths(1),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.UserGoals.Add(goal);
        }

        await _context.SaveChangesAsync();

        var result = await _tokenService.CreateTokensAsync(user, false, "guest@waohealth.com");
        return Ok(ApiResponse<AuthResponse>.Success(result, "Đăng nhập với chế độ khách ẩn danh thành công"));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Refresh([FromBody] RefreshRequest request)
    {
        var result = await _tokenService.RefreshAsync(request.RefreshToken);

        return Ok(ApiResponse<AuthResponse>.Success(result, "Làm mới token thành công"));
    }

    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<object>>> Logout([FromBody] RefreshRequest request)
    {
        await _tokenService.RevokeAsync(request.RefreshToken);

        return Ok(ApiResponse<object>.Success(null!, "Đăng xuất thành công"));
    }
}
