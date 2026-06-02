using System;
using nutrition_app_backend.Models.Foods;
using nutrition_app_backend.Models.Diaries;

namespace nutrition_app_backend.Models.Diaries
{
    public class MenuFood
    {
        public Guid Id { get; set; }
        
        public Guid MenuId { get; set; }
        
        public Guid FoodItemId { get; set; }
        
        public byte MealTypeId { get; set; } // Sáng (1), Trưa (2), Tối (3), Phụ (4)
        
        public decimal QuantityG { get; set; }

        // Navigation properties
        public Menu Menu { get; set; } = null!;
        public FoodItem FoodItem { get; set; } = null!;
        public MealType MealType { get; set; } = null!;
    }
}
