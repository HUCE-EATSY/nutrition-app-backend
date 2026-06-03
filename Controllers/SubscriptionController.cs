using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Models.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace nutrition_app_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly WaoDbContext _context;
        private readonly IConfiguration _configuration;

        public SubscriptionController(WaoDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// GET /api/Subscription/plans
        /// Trả về danh sách tất cả các gói cước (trừ FREE) để hiển thị trên UI.
        /// </summary>
        [HttpGet("plans")]
        public async Task<ActionResult<ApiResponse<object>>> GetSubscriptionPlans()
        {
            List<SubscriptionPlan> plans = await _context.SubscriptionPlans
                .Where(p => p.Code != "FREE")
                .OrderBy(p => p.Price)
                .ToListAsync();

            List<object> result = plans.Select(p => (object)new
            {
                id = p.Id,
                code = p.Code,
                name = p.Name,
                price = p.Price,
                durationDays = p.DurationDays
            }).ToList();

            return Ok(ApiResponse<object>.Success(result, "Lấy danh sách gói cước thành công"));
        }

        /// <summary>
        /// GET /api/Subscription/me
        /// Trả về trạng thái gói cước hiện tại của user.
        /// Tuyệt đối không lỗi 404. Nếu chưa mua, trả Free.
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> GetMySubscription()
        {
            Guid userId = User.GetUserId();

            // Chỉ lấy subscription Active (0) hoặc Trial (1) còn hạn
            // Loại trừ Pending (4), Cancelled (2), Expired (3)
            Subscription? subscription = await _context.Subscriptions
                .Include(s => s.Plan)
                .Where(s => s.UserId == userId
                         && (s.Status == 0 || s.Status == 1)
                         && s.CurrentPeriodEnd > DateTime.UtcNow)
                .OrderByDescending(s => s.CurrentPeriodEnd)
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                object freePlan = new
                {
                    isPremium = false,
                    plan = "Free",
                    status = "active",
                    planCode = "FREE",
                    planName = "Gói Miễn Phí"
                };
                return Ok(ApiResponse<object>.Success(freePlan, "Bạn đang sử dụng gói Miễn Phí"));
            }

            object premiumPlan = new
            {
                isPremium = true,
                plan = subscription.Plan.Code,
                status = "active",
                planCode = subscription.Plan.Code,
                planName = subscription.Plan.Name,
                expiresAt = subscription.CurrentPeriodEnd,
                statusValue = subscription.Status
            };

            return Ok(ApiResponse<object>.Success(premiumPlan, "Lấy thông tin gói cước thành công"));
        }

        /// <summary>
        /// POST /api/Subscription/vietqr/create-order
        /// Tạo đơn hàng VietQR. Sinh orderId duy nhất, tạo URL ảnh QR NAPAS, lưu subscription Pending.
        /// </summary>
        [HttpPost("vietqr/create-order")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> CreateVietQrOrder([FromBody] CreateOrderRequest request)
        {
            Guid userId = User.GetUserId();

            SubscriptionPlan? plan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.Id == request.PlanId);

            if (plan == null || plan.Code == "FREE")
            {
                return BadRequest(ApiResponse<object>.Fail("Gói cước không hợp lệ hoặc không yêu cầu thanh toán"));
            }

            string orderId = "WAOPREM" + Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();

            string bankId = _configuration["PaymentGateway:BankId"] ?? "vietinbank";
            string accountNo = _configuration["PaymentGateway:AccountNo"] ?? "102873111111";
            string accountName = _configuration["PaymentGateway:AccountName"] ?? "WAO HEALTH APP";

            decimal amount = plan.Price;
            string qrUrl = $"https://img.vietqr.io/image/{bankId}-{accountNo}-compact2.png?amount={amount}&addInfo={Uri.EscapeDataString("Thanh toan " + orderId)}";

            Subscription pendingSub = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PlanId = plan.Id,
                Status = 4, // 4 = Pending
                CurrentPeriodEnd = DateTime.UtcNow,
                LatestOrderId = orderId,
                StoreTransactionId = orderId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Subscriptions.Add(pendingSub);
            await _context.SaveChangesAsync();

            object result = new
            {
                orderId = orderId,
                qrUrl = qrUrl,
                amount = amount,
                planName = plan.Name,
                accountName = accountName,
                accountNo = accountNo,
                bankId = bankId
            };

            return Ok(ApiResponse<object>.Success(result, "Tạo đơn hàng VietQR thành công"));
        }

        /// <summary>
        /// GET /api/Subscription/vietqr/{orderId}/status
        /// Polling trạng thái đơn hàng. Frontend gọi mỗi 3 giây.
        /// </summary>
        [HttpGet("vietqr/{orderId}/status")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> GetOrderStatus(string orderId)
        {
            Subscription? subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.LatestOrderId == orderId);

            if (subscription == null)
            {
                return Ok(ApiResponse<object>.Success(new { status = "PENDING" }, "Đơn hàng đang được xử lý"));
            }

            string statusStr = "PENDING";
            if (subscription.Status == 0)
            {
                statusStr = "PAID";
            }
            else if (subscription.Status == 2 || subscription.Status == 3)
            {
                statusStr = "FAILED";
            }

            return Ok(ApiResponse<object>.Success(new { status = statusStr }, "Lấy trạng thái đơn hàng thành công"));
        }

        /// <summary>
        /// POST /api/Subscription/vietqr/callback
        /// Webhook Callback từ cổng thanh toán (PayOS/SePay cho Live, nội bộ cho Mock).
        /// Xác thực chữ ký HMAC-SHA256 ở chế độ Live.
        /// Lưu event vào subscription_events, cập nhật subscription sang Active.
        /// </summary>
        [HttpPost("vietqr/callback")]
        public async Task<IActionResult> VietQrCallback()
        {
            string requestBody = string.Empty;
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            string orderId = string.Empty;
            bool isMock = (_configuration["PaymentGateway:Mode"] ?? "Mock") == "Mock";

            try
            {
                using (JsonDocument doc = JsonDocument.Parse(requestBody))
                {
                    JsonElement root = doc.RootElement;

                    if (isMock)
                    {
                        // Chế độ Mock: đọc trực tiếp orderId từ payload
                        if (root.TryGetProperty("orderId", out JsonElement orderIdProp))
                        {
                            orderId = orderIdProp.GetString() ?? string.Empty;
                        }
                        else if (root.TryGetProperty("data", out JsonElement dataEl)
                              && dataEl.TryGetProperty("description", out JsonElement descEl))
                        {
                            string desc = descEl.GetString() ?? string.Empty;
                            int idx = desc.IndexOf("WAOPREM");
                            if (idx >= 0 && desc.Length >= idx + 17)
                            {
                                orderId = desc.Substring(idx, 17);
                            }
                        }
                    }
                    else
                    {
                        // Chế độ Live: xác thực chữ ký HMAC-SHA256
                        if (!root.TryGetProperty("data", out JsonElement dataEl) || !root.TryGetProperty("signature", out JsonElement sigEl))
                        {
                            return BadRequest("Invalid payload structure for PayOS");
                        }

                        string receivedSignature = sigEl.GetString() ?? string.Empty;
                        string secretKey = _configuration["PaymentGateway:SecretKey"] ?? string.Empty;

                        string dataSignString = BuildPayOsSignString(dataEl);
                        string calculatedSignature = ComputeHmacSha256(dataSignString, secretKey);

                        if (calculatedSignature != receivedSignature)
                        {
                            return Unauthorized("Invalid PayOS signature");
                        }

                        if (dataEl.TryGetProperty("description", out JsonElement descEl))
                        {
                            string desc = descEl.GetString() ?? string.Empty;
                            int idx = desc.IndexOf("WAOPREM");
                            if (idx >= 0 && desc.Length >= idx + 17)
                            {
                                orderId = desc.Substring(idx, 17);
                            }
                        }
                        else if (dataEl.TryGetProperty("orderCode", out JsonElement orderCodeEl))
                        {
                            string orderCodeStr = orderCodeEl.ValueKind == JsonValueKind.Number ? orderCodeEl.GetRawText() : orderCodeEl.GetString() ?? string.Empty;
                            orderId = orderCodeStr;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error parsing payload: " + ex.Message);
            }

            if (string.IsNullOrEmpty(orderId))
            {
                return BadRequest("Could not extract orderId from payload");
            }

            Subscription? subscription = await _context.Subscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.LatestOrderId == orderId);

            if (subscription == null)
            {
                return NotFound("Subscription order not found");
            }

            // Lưu vết event vào subscription_events (Append-only)
            SubscriptionEvent subEvent = new SubscriptionEvent
            {
                Id = Guid.NewGuid(),
                SubscriptionId = subscription.Id,
                Provider = isMock ? "mock" : "payos",
                EventType = "vietqr_callback",
                RawPayload = requestBody,
                ReceivedAt = DateTime.UtcNow
            };

            _context.SubscriptionEvents.Add(subEvent);

            // Cập nhật subscription: Active + cộng thêm ngày theo plan
            subscription.Status = 0; // 0 = Active
            subscription.CurrentPeriodEnd = DateTime.UtcNow.AddDays(subscription.Plan.DurationDays);
            subscription.UpdatedAt = DateTime.UtcNow;

            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Premium activated successfully" });
        }

        [HttpPost("webhook/apple")]
        public async Task<IActionResult> AppleWebhook()
        {
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                string body = await reader.ReadToEndAsync();

                SubscriptionEvent subEvent = new SubscriptionEvent
                {
                    Provider = "apple",
                    EventType = "webhook",
                    RawPayload = body
                };

                _context.SubscriptionEvents.Add(subEvent);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost("webhook/google")]
        public async Task<IActionResult> GoogleWebhook()
        {
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                string body = await reader.ReadToEndAsync();

                SubscriptionEvent subEvent = new SubscriptionEvent
                {
                    Provider = "google",
                    EventType = "webhook",
                    RawPayload = body
                };

                _context.SubscriptionEvents.Add(subEvent);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        private string BuildPayOsSignString(JsonElement dataEl)
        {
            List<string> keyValuePairs = new List<string>();
            foreach (JsonProperty prop in dataEl.EnumerateObject())
            {
                string key = prop.Name;
                JsonElement val = prop.Value;
                string valueStr = string.Empty;

                if (val.ValueKind == JsonValueKind.String)
                {
                    valueStr = val.GetString() ?? string.Empty;
                }
                else if (val.ValueKind == JsonValueKind.Number)
                {
                    valueStr = val.GetRawText();
                }
                else if (val.ValueKind == JsonValueKind.True || val.ValueKind == JsonValueKind.False)
                {
                    valueStr = val.GetBoolean().ToString().ToLower();
                }
                else if (val.ValueKind == JsonValueKind.Null)
                {
                    valueStr = string.Empty;
                }
                else
                {
                    valueStr = val.GetRawText();
                }

                keyValuePairs.Add($"{key}={valueStr}");
            }

            keyValuePairs.Sort(StringComparer.Ordinal);
            return string.Join("&", keyValuePairs);
        }

        private string ComputeHmacSha256(string message, string secret)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(secret);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            using (HMACSHA256 hmac = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmac.ComputeHash(messageBytes);
                return Convert.ToHexString(hashmessage).ToLower();
            }
        }
    }

    public class CreateOrderRequest
    {
        public int PlanId { get; set; }
    }
}
