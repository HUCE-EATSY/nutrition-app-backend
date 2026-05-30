using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Notifications;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.Notification;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/notifications")]
[AllowAnonymous]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Lấy danh sách cài đặt thông báo của user
    /// </summary>
    [HttpGet("settings")]
    public async Task<ActionResult<ApiResponse<List<UserNotificationSettingResponse>>>> GetNotificationSettings()
    {
        var userId = User.GetUserId();
        var result = await _notificationService.GetNotificationSettingsAsync(userId);

        return Ok(ApiResponse<List<UserNotificationSettingResponse>>.Success(result, "Lấy cài đặt thông báo thành công"));
    }

    /// <summary>
    /// Cập nhật cài đặt thông báo
    /// </summary>
    [HttpPut("settings")]
    public async Task<ActionResult<ApiResponse<UserNotificationSettingResponse>>> UpdateNotificationSetting(
        [FromBody] UpdateNotificationSettingRequest request)
    {
        var userId = User.GetUserId();
        var result = await _notificationService.UpdateNotificationSettingAsync(userId, request);

        return Ok(ApiResponse<UserNotificationSettingResponse>.Success(result, "Cập nhật cài đặt thành công"));
    }

    /// <summary>
    /// Lấy danh sách thông báo
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<NotificationResponse>>>> GetNotifications(
        [FromQuery] bool? isRead,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.GetUserId();
        var result = await _notificationService.GetNotificationsAsync(userId, isRead, page, pageSize);

        return Ok(ApiResponse<List<NotificationResponse>>.Success(result, "Lấy danh sách thông báo thành công"));
    }

    /// <summary>
    /// Đánh dấu thông báo đã đọc
    /// </summary>
    [HttpPost("mark-as-read")]
    public async Task<ActionResult<ApiResponse<int>>> MarkAsRead([FromBody] MarkAsReadRequest request)
    {
        var userId = User.GetUserId();
        var count = await _notificationService.MarkAsReadAsync(userId, request.NotificationIds);

        return Ok(ApiResponse<int>.Success(count, $"Đã đánh dấu {count} thông báo"));
    }

    /// <summary>
    /// Đánh dấu tất cả thông báo đã đọc
    /// </summary>
    [HttpPost("mark-all-as-read")]
    public async Task<ActionResult<ApiResponse<int>>> MarkAllAsRead()
    {
        var userId = User.GetUserId();
        var count = await _notificationService.MarkAllAsReadAsync(userId);

        return Ok(ApiResponse<int>.Success(count, $"Đã đánh dấu {count} thông báo"));
    }

    /// <summary>
    /// Xóa thông báo
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteNotification(Guid id)
    {
        var userId = User.GetUserId();
        await _notificationService.DeleteNotificationAsync(userId, id);

        return Ok(ApiResponse<object>.Success(null!, "Xóa thông báo thành công"));
    }

    /// <summary>
    /// Lấy số lượng thông báo chưa đọc
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
    {
        var userId = User.GetUserId();
        var count = await _notificationService.GetUnreadCountAsync(userId);

        return Ok(ApiResponse<int>.Success(count, "Lấy số lượng thông báo chưa đọc thành công"));
    }

    /// <summary>
    /// Đăng ký Device Token cho Push Notification
    /// </summary>
    [HttpPost("register-token")]
    public async Task<ActionResult<ApiResponse<object>>> RegisterToken([FromBody] RegisterDeviceTokenRequest request)
    {
        var userId = User.GetUserId();
        await _notificationService.RegisterDeviceTokenAsync(userId, request);

        return Ok(ApiResponse<object>.Success(null!, "Đăng ký token thành công"));
    }
}
