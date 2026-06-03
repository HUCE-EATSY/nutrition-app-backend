using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Diaries
{
    public class CreateMenuFoodRequest
    {
        [Required]
        [JsonPropertyName("foodItemId")]
        public Guid FoodItemId { get; set; }

        [Required]
        [Range(1, 4)]
        [JsonPropertyName("mealTypeId")]
        public byte MealTypeId { get; set; } // 1=Sáng, 2=Trưa, 3=Tối, 4=Phụ

        [Required]
        [Range(0.1, 99999.9)]
        [JsonPropertyName("quantityG")]
        public decimal QuantityG { get; set; }
    }

    public class CreateMenuRequest
    {
        [Required]
        [MaxLength(100)]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [MaxLength(2000)]
        [JsonPropertyName("coverImageUrl")]
        public string? CoverImageUrl { get; set; }

        [Required]
        [JsonPropertyName("foods")]
        public List<CreateMenuFoodRequest> Foods { get; set; } = new List<CreateMenuFoodRequest>();
    }
}
