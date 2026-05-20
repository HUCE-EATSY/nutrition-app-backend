using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using nutrition_app_backend.Services.Subscriptions;
using System.Security.Claims;

namespace nutrition_app_backend.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequiresPremiumAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // var userIdClaim = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        // if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        // {
        //     context.Result = new UnauthorizedResult();
        //     return;
        // }
        
        var userId = Guid.Empty; // Mock for testing since Auth is commented out currently

        var subscriptionService = context.HttpContext.RequestServices.GetRequiredService<ISubscriptionService>();
        var sub = await subscriptionService.GetSubscriptionAsync(userId);

        if (sub == null || (sub.Status != "active" && sub.Status != "trialing") || sub.CurrentPeriodEnd <= DateTime.UtcNow)
        {
            context.Result = new StatusCodeResult(403); // Forbidden
        }
    }
}
