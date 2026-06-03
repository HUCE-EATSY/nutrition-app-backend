using System;
using System.Collections.Generic;
using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Models.Diaries
{
    public class Menu
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = null!;
        
        public string? Description { get; set; }
        
        public string? CoverImageUrl { get; set; }
        
        public Guid UserId { get; set; }
        
        public decimal TotalCalories { get; set; }
        
        public decimal TotalProtein { get; set; }
        
        public decimal TotalCarbs { get; set; }
        
        public decimal TotalFat { get; set; }
        
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<MenuFood> MenuFoods { get; set; } = new List<MenuFood>();
    }
}
