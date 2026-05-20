using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.Extensions;

namespace nutrition_app_backend.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequiresPremiumAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var userPrincipal = httpContext.User;

        if (userPrincipal?.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        Guid userId;
        try
        {
            userId = userPrincipal.GetUserId();
        }
        catch
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var dbContext = httpContext.RequestServices.GetRequiredService<WaoDbContext>();

        var activeSub = await dbContext.Subscriptions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CurrentPeriodEnd)
            .FirstOrDefaultAsync();

        if (activeSub == null)
        {
            context.Result = new ObjectResult(new { message = "Premium subscription required." }) { StatusCode = 403 };
            return;
        }

        // Active status includes:
        // 0: Active, 1: Trialing, 2: Cancelled (but still valid before CurrentPeriodEnd)
        bool isPremium = activeSub.Status == 0 || activeSub.Status == 1 || (activeSub.Status == 2 && activeSub.CurrentPeriodEnd > DateTime.UtcNow);

        if (!isPremium)
        {
            context.Result = new ObjectResult(new { message = "Premium subscription required." }) { StatusCode = 403 };
            return;
        }

        await next();
    }
}
