namespace nutrition_app_backend.Services.Admin;

using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Admin;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Models;

public class AdminSubscriptionService : IAdminSubscriptionService
{
    private readonly WaoDbContext _dbContext;

    public AdminSubscriptionService(WaoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<object> GetAllSubscriptionsAsync(
        int page, int pageSize, string? search, string? status, int? planId)
    {
        var query = _dbContext.Subscriptions
            .Include(s => s.User)
                .ThenInclude(u => u.Profile)
            .Include(s => s.Plan)
            .AsQueryable();

        // Search by user name or order ID
        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(s => 
                (s.User.Profile != null && s.User.Profile.DisplayName != null && 
                 s.User.Profile.DisplayName.ToLower().Contains(searchLower)) ||
                (s.StoreTransactionId != null && s.StoreTransactionId.ToLower().Contains(searchLower)));
        }

        // Filter by status
        if (!string.IsNullOrEmpty(status))
        {
            switch (status.ToLower())
            {
                case "active":
                    query = query.Where(s => s.Status == 0 && s.CurrentPeriodEnd > DateTime.UtcNow);
                    break;
                case "trialing":
                    query = query.Where(s => s.Status == 1 && s.CurrentPeriodEnd > DateTime.UtcNow);
                    break;
                case "expired":
                    query = query.Where(s => s.CurrentPeriodEnd <= DateTime.UtcNow);
                    break;
                case "cancelled":
                    query = query.Where(s => s.Status == 2);
                    break;
            }
        }

        // Filter by plan
        if (planId.HasValue)
        {
            query = query.Where(s => s.PlanId == planId.Value);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var subscriptions = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SubscriptionDto
            {
                Id = s.Id,
                UserId = s.UserId,
                UserDisplayName = s.User.Profile != null ? s.User.Profile.DisplayName ?? "Unknown" : "Unknown",
                PlanName = s.Plan.Name,
                PlanCode = s.Plan.Code,
                Price = s.Plan.Price,
                Status = GetStatusString(s.Status, s.CurrentPeriodEnd),
                CurrentPeriodEnd = s.CurrentPeriodEnd,
                CreatedAt = s.CreatedAt,
                OrderId = s.StoreTransactionId
            })
            .ToListAsync();

        return new
        {
            items = subscriptions,
            totalCount = totalCount,
            page = page,
            pageSize = pageSize,
            totalPages = totalPages
        };
    }

    public async Task<IEnumerable<SubscriptionDto>> GetUserSubscriptionsAsync(Guid userId)
    {
        var subscriptions = await _dbContext.Subscriptions
            .Include(s => s.User)
                .ThenInclude(u => u.Profile)
            .Include(s => s.Plan)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SubscriptionDto
            {
                Id = s.Id,
                UserId = s.UserId,
                UserDisplayName = s.User.Profile != null ? s.User.Profile.DisplayName ?? "Unknown" : "Unknown",
                PlanName = s.Plan.Name,
                PlanCode = s.Plan.Code,
                Price = s.Plan.Price,
                Status = GetStatusString(s.Status, s.CurrentPeriodEnd),
                CurrentPeriodEnd = s.CurrentPeriodEnd,
                CreatedAt = s.CreatedAt,
                OrderId = s.StoreTransactionId
            })
            .ToListAsync();

        return subscriptions;
    }

    public async Task<SubscriptionDto> GrantPremiumAsync(Guid userId, GrantPremiumRequest request)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        
        try
        {
            // Kiểm tra user tồn tại
            var user = await _dbContext.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user == null)
                throw new NotFoundException("User not found");

            // Kiểm tra plan hợp lệ
            var plan = await _dbContext.SubscriptionPlans.FindAsync(request.PlanId);
            if (plan == null || plan.Code == "FREE")
                throw new BusinessException("INVALID_PLAN", "Invalid premium plan");

            // Kiểm tra đã có premium active
            var now = DateTime.UtcNow;
            var activeSubscription = await _dbContext.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && 
                                        (s.Status == 0 || s.Status == 1) && 
                                        s.CurrentPeriodEnd > now);

            if (activeSubscription != null)
                throw new BusinessException("ALREADY_PREMIUM", "User already has active premium subscription");

            // Tính ngày hết hạn
            var durationDays = request.DurationDays ?? plan.DurationDays;
            var periodEnd = now.AddDays(durationDays);

            // Tạo subscription mới
            var subscription = new nutrition_app_backend.Models.Users.Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PlanId = request.PlanId,
                Status = 0, // Active
                CurrentPeriodEnd = periodEnd,
                StoreTransactionId = $"ADMIN_GRANT_{Guid.NewGuid().ToString("N")[..8]}",
                CreatedAt = now,
                UpdatedAt = now
            };

            _dbContext.Subscriptions.Add(subscription);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new SubscriptionDto
            {
                Id = subscription.Id,
                UserId = subscription.UserId,
                UserDisplayName = user.Profile?.DisplayName ?? "Unknown",
                PlanName = plan.Name,
                PlanCode = plan.Code,
                Price = plan.Price,
                Status = "Active",
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                CreatedAt = subscription.CreatedAt,
                OrderId = subscription.StoreTransactionId
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> RevokePremiumAsync(Guid userId)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        
        try
        {
            var now = DateTime.UtcNow;
            var activeSubscription = await _dbContext.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && 
                                        (s.Status == 0 || s.Status == 1) && 
                                        s.CurrentPeriodEnd > now);

            if (activeSubscription == null)
                throw new NotFoundException("No active premium subscription found");

            // Set status to Cancelled, không xóa record để giữ lịch sử
            activeSubscription.Status = 2; // Cancelled
            activeSubscription.UpdatedAt = now;

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<SubscriptionDto> ExtendPremiumAsync(Guid userId, ExtendPremiumRequest request)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        
        try
        {
            var now = DateTime.UtcNow;
            var activeSubscription = await _dbContext.Subscriptions
                .Include(s => s.User)
                    .ThenInclude(u => u.Profile)
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.UserId == userId && 
                                        (s.Status == 0 || s.Status == 1) && 
                                        s.CurrentPeriodEnd > now);

            if (activeSubscription == null)
                throw new NotFoundException("No active premium subscription found");

            if (request.AdditionalDays <= 0)
                throw new BusinessException("INVALID_DAYS", "Additional days must be positive");

            // Gia hạn thêm
            activeSubscription.CurrentPeriodEnd = activeSubscription.CurrentPeriodEnd.AddDays(request.AdditionalDays);
            activeSubscription.UpdatedAt = now;

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new SubscriptionDto
            {
                Id = activeSubscription.Id,
                UserId = activeSubscription.UserId,
                UserDisplayName = activeSubscription.User.Profile?.DisplayName ?? "Unknown",
                PlanName = activeSubscription.Plan.Name,
                PlanCode = activeSubscription.Plan.Code,
                Price = activeSubscription.Plan.Price,
                Status = GetStatusString(activeSubscription.Status, activeSubscription.CurrentPeriodEnd),
                CurrentPeriodEnd = activeSubscription.CurrentPeriodEnd,
                CreatedAt = activeSubscription.CreatedAt,
                OrderId = activeSubscription.StoreTransactionId
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<SubscriptionStatsDto> GetSubscriptionStatsAsync()
    {
        var now = DateTime.UtcNow;
        var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

        // Total users từng có premium (không bao gồm FREE plan)
        var totalPremium = await _dbContext.Subscriptions
            .Where(s => s.PlanId != 1) // Không phải FREE
            .Select(s => s.UserId)
            .Distinct()
            .CountAsync();

        // Active premium users (không tính FREE plan)
        var activePremium = await _dbContext.Subscriptions
            .Where(s => s.PlanId != 1 && (s.Status == 0 || s.Status == 1) && s.CurrentPeriodEnd > now)
            .Select(s => s.UserId)
            .Distinct()
            .CountAsync();

        // Expired premium users
        var expiredPremium = await _dbContext.Subscriptions
            .Where(s => s.CurrentPeriodEnd <= now && s.PlanId != 1)
            .Select(s => s.UserId)
            .Distinct()
            .CountAsync();

        // Total revenue
        var totalRevenue = await _dbContext.Subscriptions
            .Include(s => s.Plan)
            .Where(s => s.PlanId != 1 && 
                       s.StoreTransactionId != null)
            .SumAsync(s => s.Plan.Price);

        // Monthly revenue
        var monthlyRevenue = await _dbContext.Subscriptions
            .Include(s => s.Plan)
            .Where(s => s.PlanId != 1 && 
                       s.CreatedAt >= firstDayOfMonth && 
                       s.StoreTransactionId != null)
            .SumAsync(s => s.Plan.Price);

        return new SubscriptionStatsDto
        {
            TotalPremium = totalPremium,
            ActivePremium = activePremium,
            ExpiredPremium = expiredPremium,
            TotalRevenue = totalRevenue,
            MonthlyRevenue = monthlyRevenue
        };
    }

    public async Task<bool> IsUserPremiumAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        return await _dbContext.Subscriptions
            .AnyAsync(s => s.UserId == userId && 
                          (s.Status == 0 || s.Status == 1) && 
                          s.CurrentPeriodEnd > now);
    }

    private static string GetStatusString(byte status, DateTime periodEnd)
    {
        if (periodEnd <= DateTime.UtcNow)
            return "Expired";
            
        return status switch
        {
            0 => "Active",
            1 => "Trialing",
            2 => "Cancelled",
            3 => "Expired",
            4 => "Pending",
            _ => "Unknown"
        };
    }
}
