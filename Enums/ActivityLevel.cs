namespace nutrition_app_backend.Enums;

/// <summary>
/// Mức độ vận động của người dùng - dùng để tính TDEE
/// </summary>
public enum ActivityLevel : byte
{
    /// <summary>
    /// Ít vận động (làm văn phòng, không tập) - BMR × 1.2
    /// </summary>
    Sedentary = 1,
    
    /// <summary>
    /// Vận động nhẹ (tập 1-3 ngày/tuần) - BMR × 1.375
    /// </summary>
    LightlyActive = 2,
    
    /// <summary>
    /// Vận động vừa (tập 3-5 ngày/tuần) - BMR × 1.55
    /// </summary>
    ModeratelyActive = 3,
    
    /// <summary>
    /// Vận động nặng (tập 6-7 ngày/tuần) - BMR × 1.725
    /// </summary>
    VeryActive = 4,
    
    /// <summary>
    /// Vận động rất nặng (lao động chân tay, tập cường độ cao) - BMR × 1.9
    /// </summary>
    ExtraActive = 5
}
