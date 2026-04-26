using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Auth;
using nutrition_app_backend.Services.Auth;
namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("google")]
    public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginRequest request)
    {
        var result = await _authService.LoginWithGoogleAsync(request);
        
        if (result == null)
        {
            var errorResponse = ApiResponse<object>.Fail("Token Google không hợp lệ hoặc đã hết hạn.", "401");
            return Unauthorized(errorResponse);
        }

        var successResponse = ApiResponse<AuthResponse>.Success(result, "Đăng nhập thành công");
        return Ok(successResponse); 
    }
}