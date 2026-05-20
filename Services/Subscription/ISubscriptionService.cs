using System;
using System.Threading.Tasks;
using nutrition_app_backend.DTOs.Users;

namespace nutrition_app_backend.Services.Subscriptions;

public interface ISubscriptionService
{
    Task<SubscriptionResponse> GetSubscriptionAsync(Guid userId);
    Task<bool> HandleAppleWebhookAsync(AppleWebhookPayload payload, string rawBody);
    Task<bool> HandleGoogleWebhookAsync(GoogleWebhookPayload payload, string rawBody);
}
