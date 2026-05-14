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

    /// <summary>
    /// Upload avatar lên Cloudinary. Gửi dưới dạng multipart/form-data với field "avatar".
    /// Trả về avatar_url mới sau khi cập nhật.
    /// </summary>
    [HttpPost("avatar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAvatar(IFormFile avatar)
    {
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
    public async Task<IActionResult> DeleteAccount()
    {
        Guid userId = User.GetUserId();
        await _userService.DeleteAccountAsync(userId);

        return NoContent();
    }
}
