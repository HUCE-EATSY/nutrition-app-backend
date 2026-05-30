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
    public async Task<IActionResult> GetAllExercises([FromQuery] int page = 1, [FromQuery] string? search = null)
    {
        var exercises = await _exerciseService.GetAllExercisesAsync(page, 20, search);
        return Ok(new { success = true, data = exercises });
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
