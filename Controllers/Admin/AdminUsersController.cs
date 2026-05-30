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
    public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] string? search = null)
    {
        var users = await _userService.GetAllUsersAsync(page, 20, search);
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
        return Ok(new { success = true });
    }
}
