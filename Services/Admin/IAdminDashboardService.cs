namespace nutrition_app_backend.Services.Admin;

using nutrition_app_backend.DTOs.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAdminDashboardService
{
    Task<AdminDashboardStatsDto> GetStatsAsync();
    Task<IEnumerable<AdminUserGrowthDto>> GetUserGrowthAsync();
}
