using System;
using System.Collections.Generic;

namespace nutrition_app_backend.DTOs.Diaries
{
    public class MenuFoodResponse
    {
        public Guid FoodItemId { get; set; }
        public string NameVi { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public byte MealTypeId { get; set; }
        public decimal QuantityG { get; set; }
        public decimal CaloriesKcal { get; set; }
        public decimal ProteinG { get; set; }
        public decimal CarbsG { get; set; }
        public decimal FatG { get; set; }
    }

    public class MenuResponse
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
        public List<MenuFoodResponse> Foods { get; set; } = new List<MenuFoodResponse>();
    }
}
