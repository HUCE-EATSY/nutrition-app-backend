using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Users;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Services.Subscriptions;

public class SubscriptionService : ISubscriptionService
{
    private readonly WaoDbContext _context;

    public SubscriptionService(WaoDbContext context)
    {
        _context = context;
    }

    public async Task<SubscriptionResponse> GetSubscriptionAsync(Guid userId)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Plan)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CurrentPeriodEnd)
            .FirstOrDefaultAsync();

        if (subscription == null)
        {
            return new SubscriptionResponse
            {
                PlanId = "free",
                PlanName = "Free Plan",
                Status = "free",
                CurrentPeriodEnd = DateTime.MaxValue
            };
        }

        string statusStr = subscription.Status switch
        {
            0 => "active",
            1 => "trialing",
            2 => "cancelled",
            3 => "expired",
            _ => "unknown"
        };

        // If expired or cancelled and expired, fallback to free
        if (subscription.Status == 3 || (subscription.Status == 2 && subscription.CurrentPeriodEnd < DateTime.UtcNow))
        {
            return new SubscriptionResponse
            {
                PlanId = "free",
                PlanName = "Free Plan",
                Status = "free",
                CurrentPeriodEnd = DateTime.MaxValue
            };
        }

        return new SubscriptionResponse
        {
            PlanId = subscription.PlanId,
            PlanName = subscription.Plan?.Name ?? "Premium Plan",
            Status = statusStr,
            CurrentPeriodEnd = subscription.CurrentPeriodEnd
        };
    }

    public async Task<bool> HandleAppleWebhookAsync(AppleWebhookPayload payload, string rawBody)
    {
        // 1. Verify signature (Apple JWS) - mockup
        if (string.IsNullOrEmpty(rawBody))
        {
            throw new BusinessException("WEBHOOK_BAD_SIGNATURE", "Invalid webhook signature.");
        }

        // Try to find subscription by StoreTransactionId
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.StoreTransactionId == payload.OriginalTransactionId || s.StoreTransactionId == payload.TransactionId);

        if (subscription == null)
        {
            if (payload.AppAccountToken == null)
            {
                // Cannot map subscription to user
                return false;
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == payload.AppAccountToken.Value);
            if (!userExists)
            {
                return false;
            }

            // Verify if plan exists, otherwise default to premium_monthly
            var planId = payload.ProductId;
            var planExists = await _context.SubscriptionPlans.AnyAsync(p => p.Id == planId);
            if (!planExists)
            {
                planId = "premium_monthly";
            }

            subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = payload.AppAccountToken.Value,
                PlanId = planId,
                Status = 1, // Trialing by default or Active
                CurrentPeriodEnd = payload.ExpiresDate,
                StoreTransactionId = payload.OriginalTransactionId ?? payload.TransactionId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Subscriptions.Add(subscription);
        }

        // 2. Log webhook event (Append-only)
        var subEvent = new SubscriptionEvent
        {
            Id = Guid.NewGuid(),
            SubscriptionId = subscription.Id,
            EventType = payload.NotificationType,
            RawPayload = rawBody,
            ReceivedAt = DateTime.UtcNow
        };
        _context.SubscriptionEvents.Add(subEvent);

        // 3. Parse event_type and update subscription status & end date
        // Mapping NotificationType: SUBSCRIBED, DID_RENEW, DID_FAIL_TO_RENEW, EXPIRED, GRACE_PERIOD_EXPIRED
        int newStatus = subscription.Status;
        if (payload.NotificationType == "SUBSCRIBED" || payload.NotificationType == "DID_RENEW")
        {
            newStatus = 0; // Active
        }
        else if (payload.NotificationType == "DID_FAIL_TO_RENEW")
        {
            newStatus = 2; // Cancelled (with grace/retry)
        }
        else if (payload.NotificationType == "EXPIRED" || payload.NotificationType == "GRACE_PERIOD_EXPIRED")
        {
            newStatus = 3; // Expired
        }

        subscription.Status = newStatus;
        subscription.CurrentPeriodEnd = payload.ExpiresDate;
        subscription.UpdatedAt = DateTime.UtcNow;

        // 4. Update user role (Premium role = 2, Free role = 1)
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == subscription.UserId);
        if (user != null)
        {
            if (newStatus == 0 || newStatus == 1 || (newStatus == 2 && subscription.CurrentPeriodEnd > DateTime.UtcNow))
            {
                user.Role = 2; // Premium
            }
            else
            {
                user.Role = 1; // Free
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HandleGoogleWebhookAsync(GoogleWebhookPayload payload, string rawBody)
    {
        // 1. Verify signature (Google JWT) - mockup
        if (string.IsNullOrEmpty(rawBody))
        {
            throw new BusinessException("WEBHOOK_BAD_SIGNATURE", "Invalid webhook signature.");
        }

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.StoreTransactionId == payload.OrderId);

        if (subscription == null)
        {
            if (payload.AppAccountToken == null)
            {
                return false;
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == payload.AppAccountToken.Value);
            if (!userExists)
            {
                return false;
            }

            var planId = payload.SubscriptionId;
            var planExists = await _context.SubscriptionPlans.AnyAsync(p => p.Id == planId);
            if (!planExists)
            {
                planId = "premium_monthly";
            }

            subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = payload.AppAccountToken.Value,
                PlanId = planId,
                Status = 0,
                CurrentPeriodEnd = payload.ExpiryTime,
                StoreTransactionId = payload.OrderId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Subscriptions.Add(subscription);
        }

        // 2. Log webhook event (Append-only)
        var subEvent = new SubscriptionEvent
        {
            Id = Guid.NewGuid(),
            SubscriptionId = subscription.Id,
            EventType = payload.EventType,
            RawPayload = rawBody,
            ReceivedAt = DateTime.UtcNow
        };
        _context.SubscriptionEvents.Add(subEvent);

        // 3. Parse event_type and update subscription status & end date
        // Mapping: SUBSCRIPTION_RECOVERED, SUBSCRIPTION_RENEWED, SUBSCRIPTION_CANCELED, SUBSCRIPTION_EXPIRED
        int newStatus = subscription.Status;
        if (payload.EventType == "SUBSCRIPTION_RECOVERED" || payload.EventType == "SUBSCRIPTION_RENEWED")
        {
            newStatus = 0; // Active
        }
        else if (payload.EventType == "SUBSCRIPTION_CANCELED")
        {
            newStatus = 2; // Cancelled
        }
        else if (payload.EventType == "SUBSCRIPTION_EXPIRED")
        {
            newStatus = 3; // Expired
        }

        subscription.Status = newStatus;
        subscription.CurrentPeriodEnd = payload.ExpiryTime;
        subscription.UpdatedAt = DateTime.UtcNow;

        // 4. Update user role
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == subscription.UserId);
        if (user != null)
        {
            if (newStatus == 0 || newStatus == 1 || (newStatus == 2 && subscription.CurrentPeriodEnd > DateTime.UtcNow))
            {
                user.Role = 2; // Premium
            }
            else
            {
                user.Role = 1; // Free
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
