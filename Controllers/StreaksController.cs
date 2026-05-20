using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Users;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.Streak;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StreaksController : ControllerBase
{
    private readonly IStreakService _streakService;

    public StreaksController(IStreakService streakService)
    {
        _streakService = streakService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<StreakResponse>>> GetStreak()
    {
        Guid userId = User.GetUserId();
        var result = await _streakService.GetStreakAsync(userId);
        return Ok(ApiResponse<StreakResponse>.Success(result, "Lấy thông tin streak thành công"));
    }

    [HttpPost("freeze")]
    public async Task<ActionResult<ApiResponse<bool>>> FreezeStreak()
    {
        Guid userId = User.GetUserId();
        var result = await _streakService.FreezeStreakAsync(userId);
        return Ok(ApiResponse<bool>.Success(result, "Freeze streak thành công"));
    }
}
