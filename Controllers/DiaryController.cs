using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Diary;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.Diary;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DiaryController : ControllerBase
{
    private readonly IDiaryService _diaryService;

    public DiaryController(IDiaryService diaryService)
    {
        _diaryService = diaryService;
    }

    /// <summary>
    /// Lấy nhật ký ăn uống trong ngày.
    /// GET /api/diary?date=2025-05-06
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetDaySummary([FromQuery] string date)
    {
        if (string.IsNullOrWhiteSpace(date))
            date = DateTime.UtcNow.ToString("yyyy-MM-dd");

        Guid userId = User.GetUserId();
        var result = await _diaryService.GetDaySummaryAsync(userId, date);
        return Ok(ApiResponse<DiaryDaySummaryResponse>.Success(result, "Lấy nhật ký thành công"));
    }

    /// <summary>
    /// Thêm bữa ăn vào nhật ký.
    /// POST /api/diary/entries
    /// </summary>
    [HttpPost("entries")]
    public async Task<IActionResult> AddEntry([FromBody] CreateDiaryEntryRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _diaryService.AddEntryAsync(userId, request);
        return CreatedAtAction(nameof(GetDaySummary),
            new { date = result.DateISO },
            ApiResponse<DiaryEntryResponse>.Success(result, "Ghi bữa ăn thành công"));
    }

    /// <summary>
    /// Xóa bữa ăn khỏi nhật ký.
    /// DELETE /api/diary/entries/{id}
    /// </summary>
    [HttpDelete("entries/{id:guid}")]
    public async Task<IActionResult> DeleteEntry([FromRoute] Guid id)
    {
        Guid userId = User.GetUserId();
        await _diaryService.DeleteEntryAsync(userId, id);
        return Ok(ApiResponse<object>.Success(null!, "Đã xóa bữa ăn"));
    }

    /// <summary>
    /// Ghi bài tập vào nhật ký.
    /// POST /api/diary/exercises
    /// </summary>
    [HttpPost("exercises")]
    public async Task<IActionResult> AddExercise([FromBody] CreateExerciseRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _diaryService.AddExerciseAsync(userId, request);
        return CreatedAtAction(nameof(GetExercises),
            new { date = result.DateISO },
            ApiResponse<ExerciseLogResponse>.Success(result, "Ghi hoạt động thành công"));
    }

    /// <summary>
    /// Lấy danh sách bài tập trong ngày.
    /// GET /api/diary/exercises?date=2025-05-06
    /// </summary>
    [HttpGet("exercises")]
    public async Task<IActionResult> GetExercises([FromQuery] string date)
    {
        if (string.IsNullOrWhiteSpace(date))
            date = DateTime.UtcNow.ToString("yyyy-MM-dd");

        Guid userId = User.GetUserId();
        var result = await _diaryService.GetExercisesAsync(userId, date);
        return Ok(ApiResponse<List<ExerciseLogResponse>>.Success(result, "Lấy danh sách hoạt động thành công"));
    }
}
