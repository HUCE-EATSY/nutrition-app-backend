namespace nutrition_app_backend.Services.Admin;

using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Admin;
using nutrition_app_backend.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AdminUserService : IAdminUserService
{
    private readonly WaoDbContext _dbContext;

    public AdminUserService(WaoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<object> GetAllUsersAsync(int page, int pageSize, string? search, string? status)
    {
        var query = _dbContext.Users
            .Where(u => u.DeletedAt == null)
            .Include(u => u.Profile)
            .Include(u => u.Subscriptions.Where(s => s.Status == 0 && s.CurrentPeriodEnd > DateTime.UtcNow))
            .ThenInclude(s => s.Plan)
            .AsQueryable();

        // Filter by search
        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(u => 
                (u.Profile != null && u.Profile.DisplayName != null && u.Profile.DisplayName.ToLower().Contains(searchLower)) ||
                (u.AuthProviders.Any(ap => ap.Email != null && ap.Email.ToLower().Contains(searchLower))));
        }

        // Filter by status
        if (!string.IsNullOrEmpty(status))
        {
            switch (status.ToLower())
            {
                case "locked":
                    query = query.Where(u => u.Status == 0);
                    break;
                case "premium":
                case "vip":
                    query = query.Where(u => u.Subscriptions.Any(s => s.Status == 0 && s.CurrentPeriodEnd > DateTime.UtcNow));
                    break;
                case "free":
                    query = query.Where(u => u.Status == 1 && !u.Subscriptions.Any(s => s.Status == 0 && s.CurrentPeriodEnd > DateTime.UtcNow));
                    break;
                // "all" or default - no additional filter
            }
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new AdminUserDto
            {
                Id = u.Id,
                Email = u.AuthProviders.FirstOrDefault() != null ? u.AuthProviders.FirstOrDefault()!.Email ?? "N/A" : "N/A",
                Name = u.Profile != null ? u.Profile.DisplayName ?? "Unknown" : "Unknown",
                CreatedAt = u.CreatedAt,
                IsActive = u.Status == 1,
                IsLocked = u.Status == 0,
                PremiumPackageId = u.Subscriptions
                    .Where(s => s.Status == 0 && s.CurrentPeriodEnd > DateTime.UtcNow)
                    .Select(s => (int?)s.PlanId)
                    .FirstOrDefault(),
                PremiumPackageName = u.Subscriptions
                    .Where(s => s.Status == 0 && s.CurrentPeriodEnd > DateTime.UtcNow)
                    .Select(s => s.Plan.Name)
                    .FirstOrDefault(),
                PremiumExpiresAt = u.Subscriptions
                    .Where(s => s.Status == 0 && s.CurrentPeriodEnd > DateTime.UtcNow)
                    .Select(s => (DateTime?)s.CurrentPeriodEnd)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return new
        {
            items = users,
            totalCount = totalCount,
            page = page,
            pageSize = pageSize,
            totalPages = totalPages
        };
    }

    public async Task<AdminUserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _dbContext.Users
            .Include(u => u.Profile)
            .Include(u => u.AuthProviders)
            .Include(u => u.Subscriptions.Where(s => s.Status == 0 && s.CurrentPeriodEnd > DateTime.UtcNow))
            .ThenInclude(s => s.Plan)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return null;

        var activeSubscription = user.Subscriptions
            .FirstOrDefault(s => s.Status == 0 && s.CurrentPeriodEnd > DateTime.UtcNow);

        return new AdminUserDto
        {
            Id = user.Id,
            Email = user.AuthProviders.FirstOrDefault()?.Email ?? "N/A",
            Name = user.Profile?.DisplayName ?? "Unknown",
            CreatedAt = user.CreatedAt,
            IsActive = user.Status == 1,
            IsLocked = user.Status == 0,
            PremiumPackageId = activeSubscription?.PlanId,
            PremiumPackageName = activeSubscription?.Plan?.Name,
            PremiumExpiresAt = activeSubscription?.CurrentPeriodEnd
        };
    }

    public async Task<AdminUserDto> ToggleUserLockAsync(Guid id)
    {
        var user = await _dbContext.Users
            .Include(u => u.Profile)
            .Include(u => u.AuthProviders)
            .Include(u => u.Subscriptions.Where(s => s.Status == 0 && s.CurrentPeriodEnd > DateTime.UtcNow))
            .ThenInclude(s => s.Plan)
            .FirstOrDefaultAsync(u => u.Id == id);
            
        if (user == null) throw new NotFoundException("User not found.");

        user.Status = (byte)(user.Status == 1 ? 0 : 1);
        user.UpdatedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync();

        var activeSubscription = user.Subscriptions
            .FirstOrDefault(s => s.Status == 0 && s.CurrentPeriodEnd > DateTime.UtcNow);

        return new AdminUserDto
        {
            Id = user.Id,
            Email = user.AuthProviders.FirstOrDefault()?.Email ?? "N/A",
            Name = user.Profile?.DisplayName ?? "Unknown",
            CreatedAt = user.CreatedAt,
            IsActive = user.Status == 1,
            IsLocked = user.Status == 0,
            PremiumPackageId = activeSubscription?.PlanId,
            PremiumPackageName = activeSubscription?.Plan?.Name,
            PremiumExpiresAt = activeSubscription?.CurrentPeriodEnd
        };
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null) throw new NotFoundException("User not found.");

        user.DeletedAt = DateTime.UtcNow;
        user.Status = 0; // Lock the user as well
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task<AdminUserStatsDto> GetUserStatsAsync()
    {
        var totalUsers = await _dbContext.Users.CountAsync(u => u.DeletedAt == null);
        var lockedUsers = await _dbContext.Users.CountAsync(u => u.Status == 0 && u.DeletedAt == null);
        
        var now = DateTime.UtcNow;
        var premiumUsers = await _dbContext.Users
            .CountAsync(u => u.DeletedAt == null && u.Subscriptions.Any(s => s.Status == 0 && s.CurrentPeriodEnd > now));
        
        var freeUsers = totalUsers - premiumUsers - lockedUsers;

        return new AdminUserStatsDto
        {
            Total = totalUsers,
            Premium = premiumUsers,
            Free = freeUsers,
            Locked = lockedUsers
        };
    }
}
