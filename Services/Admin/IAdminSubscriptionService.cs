namespace nutrition_app_backend.Services.Admin;

using nutrition_app_backend.DTOs.Admin;

public interface IAdminSubscriptionService
{
    // Lấy danh sách subscriptions với pagination
    Task<object> GetAllSubscriptionsAsync(
        int page, int pageSize, string? search, string? status, int? planId);
    
    // Lấy subscription history của 1 user
    Task<IEnumerable<SubscriptionDto>> GetUserSubscriptionsAsync(Guid userId);
    
    // Admin cấp Premium cho user (miễn phí)
    Task<SubscriptionDto> GrantPremiumAsync(Guid userId, GrantPremiumRequest request);
    
    // Admin thu hồi Premium
    Task<bool> RevokePremiumAsync(Guid userId);
    
    // Admin gia hạn Premium
    Task<SubscriptionDto> ExtendPremiumAsync(Guid userId, ExtendPremiumRequest request);
    
    // Thống kê Premium users
    Task<SubscriptionStatsDto> GetSubscriptionStatsAsync();
    
    // Kiểm tra user có Premium không
    Task<bool> IsUserPremiumAsync(Guid userId);
}
