using AutoMapper;
using nutrition_app_backend.DTOs.Diaries;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Models.Diaries;
using nutrition_app_backend.Models.Foods;
using nutrition_app_backend.Services.Storage;

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
        if (src.Source == 3)
            return src.ThumbnailUrl; // Community: full URL đã lưu sẵn

        if (src.ActiveImage == null)
            return null; // Official nhưng chưa có ảnh

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
        if (src.FoodItem.Source == 3)
            return src.FoodItem.ThumbnailUrl;

        if (src.FoodItem.ActiveImage == null)
            return null;

        return _storage.BuildUrl(src.FoodItem.ActiveImage.StoragePath);
    }
}
