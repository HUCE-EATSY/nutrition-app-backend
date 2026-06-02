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

    public async Task<IEnumerable<AdminUserDto>> GetAllUsersAsync(int page, int pageSize, string? search)
    {
        var query = _dbContext.Users.Include(u => u.Profile).AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(u => u.Profile != null && u.Profile.DisplayName != null && u.Profile.DisplayName.ToLower().Contains(searchLower));
        }

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new AdminUserDto
            {
                Id = u.Id,
                DisplayName = u.Profile != null ? u.Profile.DisplayName : null,
                Role = u.Role,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                IsVip = false // TODO: calculate VIP status
            })
            .ToListAsync();

        return users;
    }

    public async Task<AdminUserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _dbContext.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return null;

        return new AdminUserDto
        {
            Id = user.Id,
            DisplayName = user.Profile?.DisplayName,
            Role = user.Role,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            IsVip = false
        };
    }

    public async Task<bool> ToggleUserLockAsync(Guid id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null) throw new NotFoundException("User not found.");

        user.Status = (byte)(user.Status == 1 ? 0 : 1);
        user.UpdatedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<AdminUserStatsDto> GetUserStatsAsync()
    {
        var totalUsers = await _dbContext.Users.CountAsync();
        var lockedUsers = await _dbContext.Users.CountAsync(u => u.Status == 0);
        
        // TODO: Calculate VIP users properly when VIP system is implemented
        var vipUsers = 0;
        var freeUsers = totalUsers - vipUsers - lockedUsers;

        return new AdminUserStatsDto
        {
            Total = totalUsers,
            Vip = vipUsers,
            Free = freeUsers,
            Locked = lockedUsers
        };
    }
}
