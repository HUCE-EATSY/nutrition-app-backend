using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using nutrition_app_backend.Data;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace nutrition_app_backend.Extensions
{
    /// <summary>
    /// Attribute middleware kiểm tra user có gói Premium còn hạn.
    /// Cho phép đi tiếp nếu Status IN (Active=0, Trial=1) và CurrentPeriodEnd > NOW().
    /// Graceful downgrade: giữ nguyên quyền đến hết current_period_end dù webhook báo huỷ.
    /// Trả 403 nếu hết hạn hoặc chưa có Premium.
    /// </summary>
    public class RequiresPremiumAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string? userIdString = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            WaoDbContext? dbContext = context.HttpContext.RequestServices.GetService<WaoDbContext>();
            if (dbContext == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            DateTime now = DateTime.UtcNow;
            bool hasPremium = await dbContext.Subscriptions
                .AnyAsync(s => s.UserId == userId
                            && (s.Status == 0 || s.Status == 1)
                            && s.CurrentPeriodEnd > now);

            if (!hasPremium)
            {
                ProblemDetails problemDetails = new ProblemDetails
                {
                    Status = 403,
                    Title = "Premium Required",
                    Detail = "Bạn cần nâng cấp gói Premium để sử dụng tính năng này."
                };
                context.Result = new ObjectResult(problemDetails) { StatusCode = 403 };
                return;
            }

            await next();
        }
    }
}
