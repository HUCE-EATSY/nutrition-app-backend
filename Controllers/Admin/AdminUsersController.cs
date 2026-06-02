using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs.Admin;
using nutrition_app_backend.Services.Admin;

namespace nutrition_app_backend.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _userService;

    public AdminUsersController(IAdminUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null)
    {
        var users = await _userService.GetAllUsersAsync(page, pageSize, search, status);
        return Ok(new { success = true, data = users });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound(new { success = false, message = "User not found" });
        return Ok(new { success = true, data = user });
    }

    [HttpPut("{id}/toggle-lock")]
    public async Task<IActionResult> ToggleLock(Guid id)
    {
        var result = await _userService.ToggleUserLockAsync(id);
        return Ok(new { success = true, data = result });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _userService.GetUserStatsAsync();
        return Ok(new { success = true, data = stats });
    }
}
