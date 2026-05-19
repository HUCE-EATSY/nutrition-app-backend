using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Users;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.User;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("onboarding")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<UserGoalResponse>>> OnboardUser([FromBody] OnboardingRequest request)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Ok(ApiResponse<UserGoalResponse>.Success(null!, "Yêu cầu đăng nhập"));
        }

        Guid userId = User.GetUserId();
        var result = await _userService.OnboardUserAsync(userId, request);

        return Ok(ApiResponse<UserGoalResponse>.Success(result, "Cập nhật hồ sơ thành công"));
    }

    [HttpPut("profile")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<UserProfileResponse>>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Ok(ApiResponse<UserProfileResponse>.Success(null!, "Yêu cầu đăng nhập"));
        }

        Guid userId = User.GetUserId();
        var result = await _userService.UpdateUserProfileAsync(userId, request);

        return Ok(ApiResponse<UserProfileResponse>.Success(result, "Cập nhật thông tin thành công"));
    }

    [HttpPut("goal")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<UserGoalUpdateResponse>>> UpdateGoal([FromBody] UpdateUserGoalRequest request)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Ok(ApiResponse<UserGoalUpdateResponse>.Success(null!, "Yêu cầu đăng nhập"));
        }

        Guid userId = User.GetUserId();
        var result = await _userService.UpdateUserGoalAsync(userId, request);

        return Ok(ApiResponse<UserGoalUpdateResponse>.Success(result, "Cập nhật mục tiêu thành công"));
    }

    [HttpGet("info")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<GetUserInfoResponse>>> GetUserInfo()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            // Return mock data for anonymous users
            var mockData = new GetUserInfoResponse
            {
                UserId = Guid.Empty,
                Profile = null,
                ActiveGoal = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            return Ok(ApiResponse<GetUserInfoResponse>.Success(mockData, "Lấy thông tin thành công"));
        }

        Guid userId = User.GetUserId();
        var result = await _userService.GetUserInfoAsync(userId);

        return Ok(ApiResponse<GetUserInfoResponse>.Success(result, "Lấy thông tin thành công"));
    }

    /// <summary>
    /// Upload avatar lên Cloudinary. Gửi dưới dạng multipart/form-data với field "avatar".
    /// Trả về avatar_url mới sau khi cập nhật.
    /// </summary>
    [HttpPost("avatar")]
    [Consumes("multipart/form-data")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> UploadAvatar(IFormFile avatar)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Ok(ApiResponse<object>.Success(null!, "Yêu cầu đăng nhập"));
        }

        Guid userId = User.GetUserId();
        var avatarUrl = await _userService.UploadAvatarAsync(userId, avatar);

        return Ok(ApiResponse<object>.Success(new { avatar_url = avatarUrl }, "Cập nhật ảnh đại diện thành công"));
    }

    /// <summary>
    /// Xóa tài khoản của user đang đăng nhập (soft delete).
    /// Tất cả refresh token sẽ bị thu hồi ngay lập tức.
    /// Frontend nên xóa token cục bộ và điều hướng về màn hình đăng nhập sau khi nhận 204.
    /// </summary>
    [HttpDelete("account")]
    [AllowAnonymous]
    public async Task<ActionResult> DeleteAccount()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return NoContent();
        }

        Guid userId = User.GetUserId();
        await _userService.DeleteAccountAsync(userId);

        return NoContent();
    }
}
