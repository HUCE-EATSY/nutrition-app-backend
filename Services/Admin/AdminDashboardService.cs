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
        var sevenDaysAgo = DateTime.UtcNow.Date.AddDays(-7);
        
        var totalUsers = await _dbContext.Users.CountAsync();
        var newUsers7Days = await _dbContext.Users.CountAsync(u => u.CreatedAt >= sevenDaysAgo);
        var totalFoods = await _dbContext.FoodItems.CountAsync();
        var totalExercises = await _dbContext.Exercises.CountAsync();

        return new AdminDashboardStatsDto
        {
            TotalUsers = totalUsers,
            NewUsers7Days = newUsers7Days,
            TotalFoods = totalFoods,
            TotalExercises = totalExercises,
            ActiveVipUsers = 0, // Mock for now
            RevenueThisMonth = 0, // Mock for now
            RevenueLastMonth = 0 // Mock for now
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

        // Create an array with all 30 days to fill in gaps with 0
        var growth = new List<AdminUserGrowthDto>();
        for (int i = 29; i >= 0; i--)
        {
            var date = DateTime.UtcNow.Date.AddDays(-i);
            var match = users.FirstOrDefault(u => u.Date == date);
            
            growth.Add(new AdminUserGrowthDto
            {
                Date = date.ToString("MMM dd"), // Short date string
                Count = match?.Count ?? 0
            });
        }

        return growth;
    }
}
