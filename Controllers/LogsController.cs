using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Diaries;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.FoodLog;
using nutrition_app_backend.Services.WeightLog;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/logs")]
[Authorize]
public class LogsController : ControllerBase
{
    private readonly IFoodLogService _foodLogService;
    private readonly IWeightLogService _weightLogService;

    public LogsController(IFoodLogService foodLogService, IWeightLogService weightLogService)
    {
        _foodLogService = foodLogService;
        _weightLogService = weightLogService;
    }

    // ========== FOOD LOGS ==========

    /// <summary>
    /// Get food logs for a specific date, grouped by meal type.
    /// </summary>
    [HttpGet("food")]
    public async Task<IActionResult> GetDailyFoodLogs([FromQuery] DateOnly date)
    {
        Guid userId = User.GetUserId();
        var result = await _foodLogService.GetDailyLogsAsync(userId, date);

        return Ok(ApiResponse<DailyFoodLogsResponse>.Success(result, "Lấy nhật ký ăn uống thành công"));
    }

    /// <summary>
    /// Create a new food log entry. Macros are snapshot using Atwater formula.
    /// </summary>
    [HttpPost("food")]
    public async Task<IActionResult> CreateFoodLog([FromBody] CreateFoodLogRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _foodLogService.CreateAsync(userId, request);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<FoodLogResponse>.Success(result, "Ghi nhật ký thành công", "201"));
    }

    /// <summary>
    /// Update quantity of an existing food log. Macros are recalculated.
    /// </summary>
    [HttpPut("food/{id:long}")]
    public async Task<IActionResult> UpdateFoodLog(ulong id, [FromBody] UpdateFoodLogRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _foodLogService.UpdateAsync(userId, id, request);

        return Ok(ApiResponse<FoodLogResponse>.Success(result, "Cập nhật log thành công"));
    }

    /// <summary>
    /// Delete a food log entry. Only the owner can delete.
    /// </summary>
    [HttpDelete("food/{id:long}")]
    public async Task<IActionResult> DeleteFoodLog(ulong id)
    {
        Guid userId = User.GetUserId();
        await _foodLogService.DeleteAsync(userId, id);

        return Ok(ApiResponse<object>.Success(null, "Xóa log thành công"));
    }

    /// <summary>
    /// Get daily summary: total macros + comparison with active target.
    /// </summary>
    [HttpGet("food/summary")]
    public async Task<IActionResult> GetDailySummary([FromQuery] DateOnly date)
    {
        Guid userId = User.GetUserId();
        var result = await _foodLogService.GetDailySummaryAsync(userId, date);

        return Ok(ApiResponse<DailySummaryResponse>.Success(result, "Lấy tổng hợp thành công"));
    }

    // ========== WEIGHT LOGS ==========

    /// <summary>
    /// Create a weight log. Returns 409 if already logged for the same day.
    /// </summary>
    [HttpPost("weight")]
    public async Task<IActionResult> CreateWeightLog([FromBody] CreateWeightLogRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _weightLogService.CreateAsync(userId, request);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<WeightLogResponse>.Success(result, "Ghi cân nặng thành công", "201"));
    }

    /// <summary>
    /// Get weight log timeline for chart display. Ordered ascending by date.
    /// </summary>
    [HttpGet("weight")]
    public async Task<IActionResult> GetWeightTimeline([FromQuery] DateOnly from, [FromQuery] DateOnly to)
    {
        Guid userId = User.GetUserId();
        var result = await _weightLogService.GetTimelineAsync(userId, from, to);

        return Ok(ApiResponse<List<WeightLogResponse>>.Success(result, "Lấy lịch sử cân nặng thành công"));
    }

    /// <summary>
    /// Update an existing weight log (weight_kg and note). Returns 404 if not found, 403 if not owner.
    /// </summary>
    [HttpPut("weight/{id:long}")]
    public async Task<IActionResult> UpdateWeightLog(ulong id, [FromBody] UpdateWeightLogRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _weightLogService.UpdateAsync(userId, id, request);

        return Ok(ApiResponse<WeightLogResponse>.Success(result, "Cập nhật cân nặng thành công"));
    }
}
