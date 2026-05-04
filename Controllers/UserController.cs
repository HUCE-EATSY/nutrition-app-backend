using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Users;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.User;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("onboarding")]
    public async Task<IActionResult> OnboardUser([FromBody] OnboardingRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _userService.OnboardUserAsync(userId, request);

        return Ok(ApiResponse<UserGoalResponse>.Success(result, "Cập nhật hồ sơ thành công"));
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _userService.UpdateUserProfileAsync(userId, request);

        return Ok(ApiResponse<UserProfileResponse>.Success(result, "Cập nhật thông tin thành công"));
    }

    [HttpPut("goal")]
    public async Task<IActionResult> UpdateGoal([FromBody] UpdateUserGoalRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _userService.UpdateUserGoalAsync(userId, request);

        return Ok(ApiResponse<UserGoalUpdateResponse>.Success(result, "Cập nhật mục tiêu thành công"));
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetUserInfo()
    {
        Guid userId = User.GetUserId();
        var result = await _userService.GetUserInfoAsync(userId);

        return Ok(ApiResponse<GetUserInfoResponse>.Success(result, "Lấy thông tin thành công"));
    }
}
