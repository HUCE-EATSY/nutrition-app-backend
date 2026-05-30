namespace nutrition_app_backend.Services.Admin;

using nutrition_app_backend.DTOs.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public interface IAdminUserService
{
    Task<IEnumerable<AdminUserDto>> GetAllUsersAsync(int page, int pageSize, string? search);
    Task<AdminUserDto?> GetUserByIdAsync(Guid id);
    Task<bool> ToggleUserLockAsync(Guid id);
}
