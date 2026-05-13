using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.Services.Streaks;
using System.Security.Claims;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
// [Authorize] // Uncomment later when Auth is fully active
public class StreakController : ControllerBase
{
    private readonly IStreakService _streakService;

    public StreakController(IStreakService streakService)
    {
        _streakService = streakService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<object>>> GetMyStreak()
    {
        // var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userId = Guid.Empty; // TODO: Replace with actual User ID from token

        var streak = await _streakService.GetStreakAsync(userId);
        if (streak == null)
        {
            return Ok(ApiResponse<object>.Success(new { currentStreak = 0, longestStreak = 0, freezeCount = 0 }));
        }

        return Ok(ApiResponse<object>.Success(new
        {
            currentStreak = streak.CurrentStreak,
            longestStreak = streak.LongestStreak,
            freezeCount = streak.FreezeCount,
            lastLogDate = streak.LastLogDate
        }));
    }

    [HttpPost("freeze")]
    public async Task<ActionResult<ApiResponse<object>>> FreezeYesterday()
    {
        // var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userId = Guid.Empty; // TODO: Replace with actual User ID from token

        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var success = await _streakService.FreezeStreakAsync(userId, yesterday);

        if (!success)
        {
            return BadRequest(ApiResponse<object>.Fail("Cannot freeze streak. Either no freezes left or already frozen.", "400"));
        }

        return Ok(ApiResponse<object>.Success(null, "Streak frozen successfully for yesterday."));
    }
}
