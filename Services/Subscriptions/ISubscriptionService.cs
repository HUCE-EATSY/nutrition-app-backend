using nutrition_app_backend.Models.Subscriptions;

namespace nutrition_app_backend.Services.Subscriptions;

public interface ISubscriptionService
{
    Task<Subscription?> GetMySubscriptionAsync(Guid userId);
    Task<bool> HandleAppleWebhookAsync(string payload);
    Task<bool> HandleGoogleWebhookAsync(string payload);
}
