using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Exercises;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.Exercise;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/exercises")]
[AllowAnonymous]
public class ExercisesController : ControllerBase
{
    private readonly IExerciseService _exerciseService;

    public ExercisesController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    /// <summary>
    /// Lấy danh sách danh mục bài tập và các bài tập
    /// </summary>
    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<ExerciseCategoryResponse>>>> GetExerciseCategories()
    {
        var result = await _exerciseService.GetExerciseCategoriesAsync();
        return Ok(ApiResponse<List<ExerciseCategoryResponse>>.Success(result, "Lấy danh sách danh mục bài tập thành công"));
    }

    /// <summary>
    /// Lấy chi tiết một bài tập
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<ExerciseResponse>>> GetExercise(Guid id)
    {
        var result = await _exerciseService.GetExerciseByIdAsync(id);

        return Ok(ApiResponse<ExerciseResponse>.Success(result, "Lấy chi tiết bài tập thành công"));
    }

    /// <summary>
    /// Tạo nhật ký tập luyện mới
    /// </summary>
    [HttpPost("logs")]
    public async Task<ActionResult<ApiResponse<ExerciseLogResponse>>> CreateExerciseLog([FromBody] CreateExerciseLogRequest request)
    {
        // For anonymous users, use a mock userId
        var userId = User.Identity?.IsAuthenticated == true 
            ? User.GetUserId() 
            : Guid.Empty;
            
        if (userId == Guid.Empty)
        {
            // Anonymous users cannot create logs - return success but don't save
            return StatusCode(StatusCodes.Status201Created,
                ApiResponse<ExerciseLogResponse>.Success(null!, "Vui lòng đăng nhập để lưu nhật ký", "201"));
        }
        
        var result = await _exerciseService.CreateExerciseLogAsync(userId, request);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<ExerciseLogResponse>.Success(result, "Ghi nhật ký tập luyện thành công", "201"));
    }

    /// <summary>
    /// Lấy chi tiết một nhật ký tập luyện
    /// </summary>
    [HttpGet("logs/{id}")]
    public async Task<ActionResult<ApiResponse<ExerciseLogResponse>>> GetExerciseLog(Guid id)
    {
        var userId = User.GetUserId();
        var result = await _exerciseService.GetExerciseLogByIdAsync(userId, id);

        return Ok(ApiResponse<ExerciseLogResponse>.Success(result, "Lấy chi tiết nhật ký thành công"));
    }

    /// <summary>
    /// Cập nhật nhật ký tập luyện
    /// </summary>
    [HttpPut("logs/{id}")]
    public async Task<ActionResult<ApiResponse<ExerciseLogResponse>>> UpdateExerciseLog(Guid id, [FromBody] UpdateExerciseLogRequest request)
    {
        var userId = User.GetUserId();
        var result = await _exerciseService.UpdateExerciseLogAsync(userId, id, request);

        return Ok(ApiResponse<ExerciseLogResponse>.Success(result, "Cập nhật nhật ký thành công"));
    }

    /// <summary>
    /// Xóa nhật ký tập luyện
    /// </summary>
    [HttpDelete("logs/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteExerciseLog(Guid id)
    {
        var userId = User.GetUserId();
        await _exerciseService.DeleteExerciseLogAsync(userId, id);

        return Ok(ApiResponse<object>.Success(null!, "Xóa nhật ký thành công"));
    }

    /// <summary>
    /// Lấy tổng hợp tập luyện theo ngày
    /// </summary>
    [HttpGet("logs/daily/{date}")]
    public async Task<ActionResult<ApiResponse<DailyExerciseSummaryResponse>>> GetDailyExerciseSummary(DateOnly date)
    {
        var userId = User.GetUserId();
        var result = await _exerciseService.GetDailyExerciseSummaryAsync(userId, date);

        return Ok(ApiResponse<DailyExerciseSummaryResponse>.Success(result, "Lấy tổng hợp tập luyện thành công"));
    }

    /// <summary>
    /// Lấy lịch sử tập luyện theo khoảng thời gian
    /// </summary>
    [HttpGet("logs")]
    public async Task<ActionResult<ApiResponse<List<ExerciseLogResponse>>>> GetExerciseLogs(
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate)
    {
        var userId = User.GetUserId();
        var result = await _exerciseService.GetExerciseLogsAsync(userId, startDate, endDate);

        return Ok(ApiResponse<List<ExerciseLogResponse>>.Success(result, "Lấy lịch sử tập luyện thành công"));
    }
}
