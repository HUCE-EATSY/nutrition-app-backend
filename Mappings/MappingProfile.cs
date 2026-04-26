using AutoMapper;
using nutrition_app_backend.DTOs.Users;
using nutrition_app_backend.Models.Users;

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
    }
}
