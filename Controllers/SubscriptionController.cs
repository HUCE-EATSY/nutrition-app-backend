using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Models.Users;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace nutrition_app_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly WaoDbContext _context;

        public SubscriptionController(WaoDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> GetMySubscription()
        {
            Guid userId = User.GetUserId();

            var sub = await _context.Subscriptions
                .Include(s => s.Plan)
                .Where(s => s.UserId == userId && (s.Status == 0 || s.Status == 1))
                .OrderByDescending(s => s.CurrentPeriodEnd)
                .FirstOrDefaultAsync();

            if (sub == null || sub.CurrentPeriodEnd <= DateTime.UtcNow)
            {
                return Ok(ApiResponse<object>.Success(new { isPremium = false }, "Bạn chưa có gói Premium"));
            }

            var result = new
            {
                isPremium = true,
                planCode = sub.Plan.Code,
                planName = sub.Plan.Name,
                expiresAt = sub.CurrentPeriodEnd,
                status = sub.Status
            };

            return Ok(ApiResponse<object>.Success(result, "Lấy thông tin gói cước thành công"));
        }

        [HttpPost("webhook/apple")]
        public async Task<IActionResult> AppleWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            var subEvent = new SubscriptionEvent
            {
                Provider = "apple",
                EventType = "webhook",
                RawPayload = body
            };

            _context.SubscriptionEvents.Add(subEvent);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("webhook/google")]
        public async Task<IActionResult> GoogleWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            var subEvent = new SubscriptionEvent
            {
                Provider = "google",
                EventType = "webhook",
                RawPayload = body
            };

            _context.SubscriptionEvents.Add(subEvent);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
