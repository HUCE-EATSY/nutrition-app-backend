using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Users;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.Subscriptions;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SubscriptionResponse>>> GetSubscription()
    {
        Guid userId = User.GetUserId();
        var result = await _subscriptionService.GetSubscriptionAsync(userId);
        return Ok(ApiResponse<SubscriptionResponse>.Success(result, "Lấy thông tin subscription thành công"));
    }

    [HttpPost("webhook/apple")]
    public async Task<IActionResult> HandleAppleWebhook([FromBody] AppleWebhookPayload payload)
    {
        string rawBody = System.Text.Json.JsonSerializer.Serialize(payload);
        var success = await _subscriptionService.HandleAppleWebhookAsync(payload, rawBody);
        if (!success)
        {
            return BadRequest(new { message = "Webhook processing failed" });
        }
        return Ok();
    }

    [HttpPost("webhook/google")]
    public async Task<IActionResult> HandleGoogleWebhook([FromBody] GoogleWebhookPayload payload)
    {
        string rawBody = System.Text.Json.JsonSerializer.Serialize(payload);
        var success = await _subscriptionService.HandleGoogleWebhookAsync(payload, rawBody);
        if (!success)
        {
            return BadRequest(new { message = "Webhook processing failed" });
        }
        return Ok();
    }
}
