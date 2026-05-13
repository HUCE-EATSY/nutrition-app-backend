using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.Models.Subscriptions;

namespace nutrition_app_backend.Services.Subscriptions;

public class SubscriptionService : ISubscriptionService
{
    private readonly WaoDbContext _context;

    public SubscriptionService(WaoDbContext context)
    {
        _context = context;
    }

    public async Task<Subscription?> GetMySubscriptionAsync(Guid userId)
    {
        return await _context.Set<Subscription>()
            .Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<bool> HandleAppleWebhookAsync(string payload)
    {
        // TODO: Skeleton/Mock logic for Apple Webhook Signature Verification
        // Production will need proper JWS verification
        
        var eventId = Guid.NewGuid();
        var subEvent = new SubscriptionEvent
        {
            Id = eventId,
            SubscriptionId = Guid.Empty, // Will be parsed from payload in production
            EventType = "mock_apple_event",
            RawPayload = payload,
            ReceivedAt = DateTime.UtcNow
        };
        
        _context.Set<SubscriptionEvent>().Add(subEvent);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> HandleGoogleWebhookAsync(string payload)
    {
        // TODO: Skeleton/Mock logic for Google Webhook Signature Verification
        // Production will need proper JWT verification
        
        var eventId = Guid.NewGuid();
        var subEvent = new SubscriptionEvent
        {
            Id = eventId,
            SubscriptionId = Guid.Empty, // Will be parsed from payload in production
            EventType = "mock_google_event",
            RawPayload = payload,
            ReceivedAt = DateTime.UtcNow
        };
        
        _context.Set<SubscriptionEvent>().Add(subEvent);
        await _context.SaveChangesAsync();
        
        return true;
    }
}
