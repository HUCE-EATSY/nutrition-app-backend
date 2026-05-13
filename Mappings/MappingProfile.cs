using AutoMapper;
using nutrition_app_backend.DTOs.Users;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.DTOs.Diaries;
using nutrition_app_backend.Models.Users;
using nutrition_app_backend.Models.Foods;
using nutrition_app_backend.Models.Diaries;
namespace nutrition_app_backend.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // UserGoal -> UserGoalResponse
        CreateMap<UserGoal, UserGoalResponse>().ReverseMap();
        
        // UserGoal -> UserGoalUpdateResponse
        CreateMap<UserGoal, UserGoalUpdateResponse>();
        
        // UserProfile -> UserProfileResponse
        CreateMap<UserProfile, UserProfileResponse>().ReverseMap();

        // Foods
        CreateMap<FoodCategory, FoodCategoryDto>();
        CreateMap<FoodNutrition, FoodNutritionDto>();
        
        CreateMap<FoodItem, FoodDetailResponse>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<FoodItemImageUrlResolver>());

        CreateMap<FoodItemComponent, FoodComponentResponse>()
            .ForMember(dest => dest.ChildFoodNameVi, opt => opt.MapFrom(src => src.ChildFood.NameVi))
            .ForMember(dest => dest.ChildFoodNameEn, opt => opt.MapFrom(src => src.ChildFood.NameEn))
            .ForMember(dest => dest.CaloriesKcal, opt => opt.MapFrom(src => src.ChildFood.Nutrition.CaloriesKcal))
            .ForMember(dest => dest.ProteinG, opt => opt.MapFrom(src => src.ChildFood.Nutrition.ProteinG))
            .ForMember(dest => dest.CarbsG, opt => opt.MapFrom(src => src.ChildFood.Nutrition.CarbsG))
            .ForMember(dest => dest.FatG, opt => opt.MapFrom(src => src.ChildFood.Nutrition.FatG));

        CreateMap<MealType, MealTypeResponse>();

        // Diaries
        CreateMap<FoodLog, FoodLogResponse>()
            .ForMember(dest => dest.FoodNameVi, opt => opt.MapFrom(src => src.FoodItem.NameVi))
            .ForMember(dest => dest.FoodNameEn, opt => opt.MapFrom(src => src.FoodItem.NameEn))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<FoodLogImageUrlResolver>())
            .ForMember(dest => dest.MealTypeName, opt => opt.MapFrom(src => src.MealType.NameVi));

        CreateMap<WeightLog, WeightLogResponse>();
    }
}
