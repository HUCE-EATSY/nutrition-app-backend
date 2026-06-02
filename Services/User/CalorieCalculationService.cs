using nutrition_app_backend.Enums;

namespace nutrition_app_backend.Services.User;

/// <summary>
/// Service tính toán BMR và TDEE theo công thức Mifflin-St Jeor
/// </summary>
public class CalorieCalculationService
{
    /// <summary>
    /// Tính BMR (Basal Metabolic Rate) - Tỷ lệ trao đổi chất cơ bản
    /// </summary>
    /// <param name="weightKg">Cân nặng (kg)</param>
    /// <param name="heightCm">Chiều cao (cm)</param>
    /// <param name="age">Tuổi (năm)</param>
    /// <param name="gender">Giới tính</param>
    /// <returns>BMR (calories/ngày)</returns>
    public static decimal CalculateBMR(decimal weightKg, decimal heightCm, int age, Gender gender)
    {
        // Công thức Mifflin-St Jeor
        // BMR = (10 × W) + (6.25 × H) - (5 × A) + offset
        // Nam: offset = +5
        // Nữ: offset = -161
        
        decimal bmr = (10m * weightKg) + (6.25m * heightCm) - (5m * age);
        
        if (gender == Gender.Male)
        {
            bmr += 5m;
        }
        else if (gender == Gender.Female)
        {
            bmr -= 161m;
        }
        
        return Math.Round(bmr, 2);
    }
    
    /// <summary>
    /// Lấy hệ số hoạt động theo mức độ vận động
    /// </summary>
    public static decimal GetActivityMultiplier(ActivityLevel activityLevel)
    {
        return activityLevel switch
        {
            ActivityLevel.Sedentary => 1.2m,          // Ít vận động
            ActivityLevel.LightlyActive => 1.375m,    // Vận động nhẹ
            ActivityLevel.ModeratelyActive => 1.55m,  // Vận động vừa
            ActivityLevel.VeryActive => 1.725m,       // Vận động nặng
            ActivityLevel.ExtraActive => 1.9m,        // Vận động rất nặng
            _ => 1.2m
        };
    }
    
    /// <summary>
    /// Tính TDEE (Total Daily Energy Expenditure) - Tổng năng lượng tiêu thụ hàng ngày
    /// </summary>
    /// <param name="bmr">BMR đã tính từ CalculateBMR</param>
    /// <param name="activityLevel">Mức độ vận động</param>
    /// <returns>TDEE (calories/ngày)</returns>
    public static decimal CalculateTDEE(decimal bmr, ActivityLevel activityLevel)
    {
        decimal multiplier = GetActivityMultiplier(activityLevel);
        return Math.Round(bmr * multiplier, 2);
    }
    
    /// <summary>
    /// Tính TDEE trực tiếp từ thông tin người dùng
    /// </summary>
    public static decimal CalculateTDEE(decimal weightKg, decimal heightCm, int age, Gender gender, ActivityLevel activityLevel)
    {
        decimal bmr = CalculateBMR(weightKg, heightCm, age, gender);
        return CalculateTDEE(bmr, activityLevel);
    }
    
    /// <summary>
    /// Lấy TDEE cho tracking theo tuần (giống TDEE hàng ngày, không nhân 7)
    /// </summary>
    public static decimal CalculateWeeklyTDEE(decimal dailyTDEE)
    {
        return dailyTDEE;
    }
}
