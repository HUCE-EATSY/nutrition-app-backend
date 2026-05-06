using nutrition_app_backend.DTOs.Foods;

namespace nutrition_app_backend.Services.Food;

public interface IFoodService
{
    /// <summary>Tìm kiếm món ăn theo tên, trả về tối đa 20 kết quả.</summary>
    Task<List<FoodResponse>> SearchFoodsAsync(string? query);

    /// <summary>Tạo món ăn mới, trả về DTO của món vừa tạo.</summary>
    Task<FoodResponse> CreateFoodAsync(Guid userId, CreateFoodRequest request);

    /// <summary>Lấy chi tiết một món ăn theo ID.</summary>
    Task<FoodResponse?> GetFoodByIdAsync(Guid id);
}
