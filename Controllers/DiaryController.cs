using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
<<<<<<< HEAD
using nutrition_app_backend.DTOs.Diary;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.Diary;
=======
using nutrition_app_backend.Extensions;
using System.Security.Claims;
>>>>>>> feature/phase-2-food-db-and-logging

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
<<<<<<< HEAD
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
=======
// [Authorize] // Tạm thời comment để test
public class DiaryController : ControllerBase
{
    // Tạm thời return mock data để frontend hoạt động
    // TODO: Implement real database operations

    [HttpGet]
    public ActionResult<ApiResponse<object>> GetDiary([FromQuery] string date)
    {
        // Mock data structure
        var mockData = new
        {
            date = date,
            targetCalories = 2000,
            targetProteinGram = 120,
            targetCarbGram = 250,
            targetFatGram = 67,
            consumedCalories = 0,
            consumedProteinGram = 0,
            consumedCarbGram = 0,
            consumedFatGram = 0,
            slots = new List<object>()
        };

        return Ok(ApiResponse<object>.Success(mockData));
    }

    [HttpGet("exercises")]
    public ActionResult<ApiResponse<List<object>>> GetExercises([FromQuery] string date)
    {
        return Ok(ApiResponse<List<object>>.Success(new List<object>()));
    }

    [HttpPost("entries")]
    public ActionResult<ApiResponse<object>> CreateEntry([FromBody] CreateFoodLogDto dto)
    {
        // Tạm thời chỉ return success
        // TODO: Save to database
        
        var result = new
        {
            id = Guid.NewGuid(),
            foodId = dto.FoodId,
            dateISO = dto.DateISO,
            hour = dto.Hour,
            quantityG = dto.QuantityG,
            totalCalories = dto.TotalCalories,
            proteinGram = dto.ProteinGram,
            carbGram = dto.CarbGram,
            fatGram = dto.FatGram,
            createdAt = DateTime.UtcNow
        };

        return Ok(ApiResponse<object>.Success(result, "Đã lưu bữa ăn thành công"));
    }

    [HttpPost("exercises")]
    public ActionResult<ApiResponse<object>> CreateExercise([FromBody] object dto)
    {
        return Ok(ApiResponse<object>.Success(null, "Đã lưu hoạt động thành công"));
    }

    [HttpDelete("entries/{id}")]
    public ActionResult<ApiResponse<object>> DeleteEntry(string id)
    {
        return Ok(ApiResponse<object>.Success(null, "Đã xóa thành công"));
    }
}

// DTOs
public class CreateFoodLogDto
{
    public int FoodId { get; set; }
    public string DateISO { get; set; } = string.Empty;
    public int Hour { get; set; }
    public decimal QuantityG { get; set; }
    public decimal TotalCalories { get; set; }
    public decimal ProteinGram { get; set; }
    public decimal CarbGram { get; set; }
    public decimal FatGram { get; set; }
}
>>>>>>> feature/phase-2-food-db-and-logging
