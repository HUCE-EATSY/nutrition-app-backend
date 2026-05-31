using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace nutrition_app_backend.Extensions
{
    public class RequiresPremiumAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userIdString = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var dbContext = context.HttpContext.RequestServices.GetService<WaoDbContext>();
            if (dbContext == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            var now = DateTime.UtcNow;
            var hasPremium = await dbContext.Subscriptions
                .AnyAsync(s => s.UserId == userId 
                            && (s.Status == 0 || s.Status == 1) 
                            && s.CurrentPeriodEnd > now);

            if (!hasPremium)
            {
                var problemDetails = new ProblemDetails
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
