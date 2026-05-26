using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Users;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.HealthConnection;
using nutrition_app_backend.Enums;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/health/connections")]
[Authorize]
public class HealthConnectionsController : ControllerBase
{
    private readonly IHealthConnectionService _healthConnectionService;

    public HealthConnectionsController(IHealthConnectionService healthConnectionService)
    {
        _healthConnectionService = healthConnectionService;
    }

    /// <summary>
    /// Lấy danh sách trạng thái kết nối wearable của người dùng.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<UserHealthConnectionResponse>>>> GetConnections()
    {
        Guid userId = User.GetUserId();
        var result = await _healthConnectionService.GetConnectionsAsync(userId);

        return Ok(ApiResponse<List<UserHealthConnectionResponse>>.Success(result, "Lấy trạng thái kết nối thành công"));
    }

    /// <summary>
    /// Bật kết nối với một ứng dụng sức khỏe (Apple Health, Google Fit...)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserHealthConnectionResponse>>> Connect([FromBody] ConnectHealthRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _healthConnectionService.ConnectAsync(userId, request);

        return Ok(ApiResponse<UserHealthConnectionResponse>.Success(result, "Kết nối ứng dụng sức khỏe thành công"));
    }

    /// <summary>
    /// Ngắt kết nối ứng dụng sức khỏe.
    /// </summary>
    [HttpDelete("{provider}")]
    public async Task<ActionResult<ApiResponse<object>>> Disconnect([FromRoute] HealthProvider provider)
    {
        Guid userId = User.GetUserId();
        await _healthConnectionService.DisconnectAsync(userId, provider);

        return Ok(ApiResponse<object>.Success(null!, "Ngắt kết nối ứng dụng sức khỏe thành công"));
    }
}
