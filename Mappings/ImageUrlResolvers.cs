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
        if (src.Source == FoodSource.Community)
            return src.ThumbnailUrl; // Community: full URL đã lưu sẵn

        if (src.ActiveImage == null)
            return src.ThumbnailUrl; // Official có thể có url ảnh cào sẵn trong ThumbnailUrl

        return _storage.BuildUrl(src.ActiveImage.StoragePath); // Official: build URL từ public_id
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
        if (src.FoodItem.Source == FoodSource.Community || src.FoodItem.Source == FoodSource.OpenFoodFacts)
            return src.FoodItem.ThumbnailUrl;

        if (src.FoodItem.ActiveImage == null)
            return src.FoodItem.ThumbnailUrl; // Fallback cho OFF và official có ThumbnailUrl

        return _storage.BuildUrl(src.FoodItem.ActiveImage.StoragePath);
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
        if (src.ChildFood.Source == FoodSource.Community)
            return src.ChildFood.ThumbnailUrl;

        if (src.ChildFood.ActiveImage == null)
            return src.ChildFood.ThumbnailUrl; // Fallback nếu không có ActiveImage (chẳng hạn ảnh cào hoặc official link sẵn)

        return _storage.BuildUrl(src.ChildFood.ActiveImage.StoragePath);
    }
}
