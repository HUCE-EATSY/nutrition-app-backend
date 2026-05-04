using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Auth;
using nutrition_app_backend.Services.Auth;
using nutrition_app_backend.Services.Token;
using RefreshRequest = nutrition_app_backend.DTOs.Auth.RefreshRequest;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;

    public AuthController(
        IAuthService authService,
        ITokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    [HttpPost("google")]
    public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginRequest request)
    {
        var result = await _authService.LoginWithGoogleAsync(request);

        return Ok(ApiResponse<AuthResponse>.Success(result, "Đăng nhập thành công"));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var result = await _tokenService.RefreshAsync(request.RefreshToken);

        return Ok(ApiResponse<AuthResponse>.Success(result, "Làm mới token thành công"));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest request)
    {
        await _tokenService.RevokeAsync(request.RefreshToken);

        return Ok(ApiResponse<object>.Success(null, "Đăng xuất thành công"));
    }
}
