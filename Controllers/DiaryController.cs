using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Diaries;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.FoodLog;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/diary")]
[AllowAnonymous]
public class DiaryController : ControllerBase
{
    private readonly IFoodLogService _foodLogService;

    public DiaryController(IFoodLogService foodLogService)
    {
        _foodLogService = foodLogService;
    }

    /// <summary>
    /// Get diary summary for a specific date (for dashboard)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<DailyFoodLogsResponse>>> GetDiarySummary([FromQuery] string date)
    {
        // Parse date
        if (!DateOnly.TryParse(date, out var parsedDate))
        {
            parsedDate = DateOnly.FromDateTime(DateTime.Today);
        }

        // For anonymous users, return empty data
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            var emptyResult = new DailyFoodLogsResponse
            {
                Date = parsedDate,
                Meals = new List<MealGroupDto>()
            };
            return Ok(ApiResponse<DailyFoodLogsResponse>.Success(emptyResult, "Lấy nhật ký thành công"));
        }

        Guid userId = User.GetUserId();
        var result = await _foodLogService.GetDailyLogsAsync(userId, parsedDate);

        return Ok(ApiResponse<DailyFoodLogsResponse>.Success(result, "Lấy nhật ký thành công"));
    }
}
