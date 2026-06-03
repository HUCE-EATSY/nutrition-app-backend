using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Users;

/// <summary>
/// Response chứa thông tin tính toán calo BMR và TDEE
/// </summary>
public record CalorieCalculationResponse
{
    /// <summary>
    /// BMR - Basal Metabolic Rate (Tỷ lệ trao đổi chất cơ bản) - Calo/ngày
    /// </summary>
    [JsonPropertyName("bmr")]
    public decimal BMR { get; init; }
    
    /// <summary>
    /// TDEE - Total Daily Energy Expenditure (Tổng năng lượng tiêu thụ hàng ngày) - Calo/ngày
    /// </summary>
    [JsonPropertyName("tdee")]
    public decimal TDEE { get; init; }
    
    /// <summary>
    /// TDEE cho tracking theo tuần (giá trị giống TDEE, không nhân 7)
    /// </summary>
    [JsonPropertyName("weeklyTdee")]
    public decimal WeeklyTDEE { get; init; }
    
    /// <summary>
    /// Hệ số hoạt động được áp dụng
    /// </summary>
    [JsonPropertyName("activityMultiplier")]
    public decimal ActivityMultiplier { get; init; }
    
    /// <summary>
    /// Mô tả mức độ hoạt động
    /// </summary>
    [JsonPropertyName("activityDescription")]
    public string ActivityDescription { get; init; } = string.Empty;
}
