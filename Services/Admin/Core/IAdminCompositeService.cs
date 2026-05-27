using nutrition_app_backend.Services.Admin.FoodManagement;

namespace nutrition_app_backend.Services.Admin.Core;

public interface IAdminCompositeService
{
    IAdminFoodService Foods { get; }
    // In the future, we can add other sub-services here:
    // IAdminUserService Users { get; }
    // IAdminAnalyticsService Analytics { get; }
    // IAdminLogsService Logs { get; }
}
