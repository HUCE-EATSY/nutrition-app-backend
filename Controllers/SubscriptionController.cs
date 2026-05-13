using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.Services.Subscriptions;
using System.Security.Claims;
using System.Text.Json;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet("me")]
    // [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetMySubscription()
    {
        // var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userId = Guid.Empty; // TODO: Replace with actual User ID from token

        var sub = await _subscriptionService.GetMySubscriptionAsync(userId);
        if (sub == null || sub.Status > 1) // 1 = Trialing, 0 = Active
        {
            return Ok(ApiResponse<object>.Success(new { plan = "Free", status = "active" }));
        }

        return Ok(ApiResponse<object>.Success(new
        {
            plan = sub.Plan?.Name ?? "Unknown",
            status = sub.Status == 0 ? "active" : "trialing",
            currentPeriodEnd = sub.CurrentPeriodEnd
        }));
    }

    [HttpPost("webhook/apple")]
    public async Task<IActionResult> AppleWebhook([FromBody] JsonElement payload)
    {
        var rawJson = payload.GetRawText();
        var result = await _subscriptionService.HandleAppleWebhookAsync(rawJson);
        
        if (!result) return Unauthorized();
        return Ok();
    }

    [HttpPost("webhook/google")]
    public async Task<IActionResult> GoogleWebhook([FromBody] JsonElement payload)
    {
        var rawJson = payload.GetRawText();
        var result = await _subscriptionService.HandleGoogleWebhookAsync(rawJson);
        
        if (!result) return Unauthorized();
        return Ok();
    }
}
