using AutoMapper;
using nutrition_app_backend.DTOs.Diaries;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Models.Diaries;
using nutrition_app_backend.Models.Foods;
using nutrition_app_backend.Services.Storage;
using nutrition_app_backend.Enums;

namespace nutrition_app_backend.Mappings;

/// <summary>
/// AutoMapper IValueResolver để build image_url khi map FoodItem -> FoodDetailResponse.
/// - source = 3 (community): dùng thumbnail_url (đã là full URL từ Cloudinary, lưu nguyên)
/// - source != 3 (official): build URL từ public_id trong food_item_images.storage_path
/// </summary>
public class FoodItemImageUrlResolver : IValueResolver<FoodItem, FoodDetailResponse, string?>
{
    private readonly IStorageService _storage;

    public FoodItemImageUrlResolver(IStorageService storage)
    {
        _storage = storage;
    }

    public string? Resolve(FoodItem src, FoodDetailResponse dest, string? destMember, ResolutionContext context)
    {
        if (src.Source == FoodSource.Community 
            || src.Source == FoodSource.BarcodeCommunity 
            || src.Source == FoodSource.OpenFoodFacts)
            return src.ThumbnailUrl;

        if (src.ActiveImage == null)
            return null;

        return _storage.BuildUrl(src.ActiveImage.StoragePath);
    }
}

/// <summary>
/// AutoMapper IValueResolver để build image_url khi map FoodLog -> FoodLogResponse.
/// </summary>
public class FoodLogImageUrlResolver : IValueResolver<FoodLog, FoodLogResponse, string?>
{
    private readonly IStorageService _storage;

    public FoodLogImageUrlResolver(IStorageService storage)
    {
        _storage = storage;
    }

    public string? Resolve(FoodLog src, FoodLogResponse dest, string? destMember, ResolutionContext context)
    {
        var food = src.FoodItem;
        if (food.Source == FoodSource.Community 
            || food.Source == FoodSource.BarcodeCommunity 
            || food.Source == FoodSource.OpenFoodFacts)
            return food.ThumbnailUrl;

        if (food.ActiveImage == null)
            return null;

        return _storage.BuildUrl(food.ActiveImage.StoragePath);
    }
}

/// <summary>
/// AutoMapper IValueResolver để build image_url khi map FoodItemComponent -> FoodComponentResponse.
/// </summary>
public class FoodComponentImageUrlResolver : IValueResolver<FoodItemComponent, FoodComponentResponse, string?>
{
    private readonly IStorageService _storage;

    public FoodComponentImageUrlResolver(IStorageService storage)
    {
        _storage = storage;
    }

    public string? Resolve(FoodItemComponent src, FoodComponentResponse dest, string? destMember, ResolutionContext context)
    {
        var child = src.ChildFood;
        if (child.Source == FoodSource.Community 
            || child.Source == FoodSource.BarcodeCommunity 
            || child.Source == FoodSource.OpenFoodFacts)
            return child.ThumbnailUrl;

        if (child.ActiveImage == null)
            return null;

        return _storage.BuildUrl(child.ActiveImage.StoragePath);
    }
}
