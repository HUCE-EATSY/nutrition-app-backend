namespace nutrition_app_backend.Services.Admin;

using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly WaoDbContext _dbContext;

    public AdminDashboardService(WaoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AdminDashboardStatsDto> GetStatsAsync()
    {
        var now = DateTime.UtcNow;
        var sevenDaysAgo = now.Date.AddDays(-7);
        var thirtyDaysAgo = now.Date.AddDays(-30);
        var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        var firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);
        
        var totalUsers = await _dbContext.Users.CountAsync();
        var newUsers7Days = await _dbContext.Users.CountAsync(u => u.CreatedAt >= sevenDaysAgo);
        var newUsers30Days = await _dbContext.Users.CountAsync(u => u.CreatedAt >= thirtyDaysAgo);
        var totalFoods = await _dbContext.FoodItems.CountAsync();
        var totalExercises = await _dbContext.Exercises.CountAsync();
        
        var activePremiumUsers = await _dbContext.Subscriptions
            .Where(s => (s.Status == 0 || s.Status == 1) && s.CurrentPeriodEnd > now && s.PlanId != 1)
            .Select(s => s.UserId)
            .Distinct()
            .CountAsync();

        var revenueThisMonth = await _dbContext.Subscriptions
            .Include(s => s.Plan)
            .Where(s => s.PlanId != 1 && s.CreatedAt >= firstDayOfMonth
                && s.StoreTransactionId != null)
            .SumAsync(s => s.Plan.Price);

        var revenueLastMonth = await _dbContext.Subscriptions
            .Include(s => s.Plan)
            .Where(s => s.PlanId != 1 
                && s.CreatedAt >= firstDayOfLastMonth && s.CreatedAt < firstDayOfMonth
                && s.StoreTransactionId != null)
            .SumAsync(s => s.Plan.Price);

        return new AdminDashboardStatsDto
        {
            TotalUsers = totalUsers,
            NewUsers7Days = newUsers7Days,
            NewUsers30Days = newUsers30Days,
            TotalFoods = totalFoods,
            TotalExercises = totalExercises,
            ActivePremiumUsers = activePremiumUsers,
            RevenueThisMonth = revenueThisMonth,
            RevenueLastMonth = revenueLastMonth
        };
    }

    public async Task<IEnumerable<AdminUserGrowthDto>> GetUserGrowthAsync()
    {
        var thirtyDaysAgo = DateTime.UtcNow.Date.AddDays(-30);
        
        var users = await _dbContext.Users
            .Where(u => u.CreatedAt >= thirtyDaysAgo)
            .GroupBy(u => u.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        var premiums = await _dbContext.Subscriptions
            .Where(s => s.CreatedAt >= thirtyDaysAgo && s.PlanId != 1)
            .GroupBy(s => s.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        // Create an array with all 30 days to fill in gaps with 0
        var growth = new List<AdminUserGrowthDto>();
        for (int i = 29; i >= 0; i--)
        {
            var date = DateTime.UtcNow.Date.AddDays(-i);
            var matchUser = users.FirstOrDefault(u => u.Date == date);
            var matchPremium = premiums.FirstOrDefault(p => p.Date == date);
            
            growth.Add(new AdminUserGrowthDto
            {
                Date = date.ToString("yyyy-MM-dd"), // ISO format for frontend
                Count = matchUser?.Count ?? 0,
                PremiumCount = matchPremium?.Count ?? 0
            });
        }

        return growth;
    }
}
