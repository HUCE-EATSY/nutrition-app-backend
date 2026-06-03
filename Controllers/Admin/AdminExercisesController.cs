using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs.Admin;
using nutrition_app_backend.Services.Admin;

namespace nutrition_app_backend.Controllers.Admin;

[ApiController]
[Route("api/admin/exercises")]
public class AdminExercisesController : ControllerBase
{
    private readonly IAdminExerciseService _exerciseService;

    public AdminExercisesController(IAdminExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllExercises(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15,
        [FromQuery] string? search = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? status = null)
    {
        var exercises = await _exerciseService.GetAllExercisesAsync(page, pageSize, search, categoryId, status);
        return Ok(new { success = true, data = exercises });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _exerciseService.GetStatsAsync();
        return Ok(new { success = true, data = stats });
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _exerciseService.GetCategoriesAsync();
        return Ok(new { success = true, data = categories });
    }

    [HttpPost]
    public async Task<IActionResult> CreateExercise([FromBody] AdminExerciseCreateDto dto)
    {
        var exercise = await _exerciseService.CreateExerciseAsync(dto);
        return Ok(new { success = true, data = exercise });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExercise(Guid id, [FromBody] AdminExerciseUpdateDto dto)
    {
        var exercise = await _exerciseService.UpdateExerciseAsync(id, dto);
        return Ok(new { success = true, data = exercise });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExercise(Guid id)
    {
        await _exerciseService.DeleteExerciseAsync(id);
        return Ok(new { success = true });
    }

    [HttpPut("{id}/toggle-visibility")]
    public async Task<IActionResult> ToggleVisibility(Guid id)
    {
        await _exerciseService.ToggleVisibilityAsync(id);
        return Ok(new { success = true });
    }
}
