using System;
using nutrition_app_backend.Models.Users;
using nutrition_app_backend.Models.Foods;

namespace nutrition_app_backend.Models.Diaries
{
    public class DailyPlan
    {
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }
        
        public DateTime LogDate { get; set; } // The target planned date (YYYY-MM-DD)
        
        public Guid FoodItemId { get; set; }
        
        public byte MealTypeId { get; set; } // Sáng (1), Trưa (2), Tối (3), Phụ (4)
        
        public decimal QuantityG { get; set; }
        
        public bool IsSynced { get; set; } // False = Planned only, True = Synced to actual FoodLogs

        // Navigation properties
        public User User { get; set; } = null!;
        public FoodItem FoodItem { get; set; } = null!;
        public MealType MealType { get; set; } = null!;
    }
}
