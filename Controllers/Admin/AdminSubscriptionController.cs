namespace nutrition_app_backend.Controllers.Admin;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs.Admin;
using nutrition_app_backend.Services.Admin;
using nutrition_app_backend.Exceptions;

[ApiController]
[Route("api/admin/subscriptions")]
[Authorize(Roles = "Admin")]
public class AdminSubscriptionController : ControllerBase
{
    private readonly IAdminSubscriptionService _subscriptionService;
    private readonly ILogger<AdminSubscriptionController> _logger;

    public AdminSubscriptionController(
        IAdminSubscriptionService subscriptionService,
        ILogger<AdminSubscriptionController> logger)
    {
        _subscriptionService = subscriptionService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy tất cả subscriptions với pagination và filter
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] int? planId = null)
    {
        try
        {
            var result = await _subscriptionService.GetAllSubscriptionsAsync(
                page, pageSize, search, status, planId);
            
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all subscriptions");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Lấy subscription history của 1 user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserSubscriptions(Guid userId)
    {
        try
        {
            var subscriptions = await _subscriptionService.GetUserSubscriptionsAsync(userId);
            return Ok(new { success = true, data = subscriptions });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user subscriptions for userId: {UserId}", userId);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Admin cấp Premium cho user (miễn phí)
    /// </summary>
    [HttpPost("user/{userId}/grant")]
    public async Task<IActionResult> GrantPremium(
        Guid userId,
        [FromBody] GrantPremiumRequest request)
    {
        try
        {
            var subscription = await _subscriptionService.GrantPremiumAsync(userId, request);
            return Ok(new { success = true, data = subscription, message = "Premium granted successfully" });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error granting premium to userId: {UserId}", userId);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Admin thu hồi Premium của user
    /// </summary>
    [HttpPut("user/{userId}/revoke")]
    public async Task<IActionResult> RevokePremium(Guid userId)
    {
        try
        {
            var result = await _subscriptionService.RevokePremiumAsync(userId);
            return Ok(new { success = true, message = "Premium revoked successfully" });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking premium from userId: {UserId}", userId);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Admin gia hạn Premium cho user
    /// </summary>
    [HttpPut("user/{userId}/extend")]
    public async Task<IActionResult> ExtendPremium(
        Guid userId,
        [FromBody] ExtendPremiumRequest request)
    {
        try
        {
            var subscription = await _subscriptionService.ExtendPremiumAsync(userId, request);
            return Ok(new { success = true, data = subscription, message = "Premium extended successfully" });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extending premium for userId: {UserId}", userId);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Thống kê Premium users và revenue
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var stats = await _subscriptionService.GetSubscriptionStatsAsync();
            return Ok(new { success = true, data = stats });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subscription stats");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }
}
