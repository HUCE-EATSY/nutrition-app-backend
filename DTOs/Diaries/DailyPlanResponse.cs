using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Diaries
{
    public class DailyPlanItemResponse
    {
        public Guid Id { get; set; }
        public Guid FoodItemId { get; set; }
        public string FoodNameVi { get; set; } = null!;
        public string FoodNameEn { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public byte MealTypeId { get; set; }
        public decimal QuantityG { get; set; }
        public decimal CaloriesKcal { get; set; }
        public decimal ProteinG { get; set; }
        public decimal CarbsG { get; set; }
        public decimal FatG { get; set; }
        public bool IsSynced { get; set; }
    }

    public class DailyPlanResponse
    {
        public DateTime LogDate { get; set; }
        public List<DailyPlanItemResponse> Items { get; set; } = new List<DailyPlanItemResponse>();
    }

    public class SyncToDiaryRequest
    {
        [Required]
        [Range(1, 4)]
        [JsonPropertyName("mealTypeId")]
        public byte MealTypeId { get; set; } // 1=Sáng, 2=Trưa, 3=Tối, 4=Phụ

        [Required]
        [JsonPropertyName("date")]
        public string Date { get; set; } = null!; // YYYY-MM-DD
    }

    public class ApplyDailyPlanRequest
    {
        [Required]
        [JsonPropertyName("menuId")]
        public Guid MenuId { get; set; }

        [Required]
        [JsonPropertyName("date")]
        public string Date { get; set; } = null!; // YYYY-MM-DD
    }
}
