using nutrition_app_backend.Services.Admin.FoodManagement;

namespace nutrition_app_backend.Services.Admin.Core;

public class AdminCompositeService : IAdminCompositeService
{
    public IAdminFoodService Foods { get; }

    public AdminCompositeService(IAdminFoodService foods)
    {
        Foods = foods;
    }
}
