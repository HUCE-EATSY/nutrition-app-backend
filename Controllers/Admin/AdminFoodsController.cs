using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Admin;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Enums;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.Admin.Core;

namespace nutrition_app_backend.Controllers.Admin;

[ApiController]
[Route("api/admin/foods")]
[Authorize]
public class AdminFoodsController : ControllerBase
{
    private readonly IAdminCompositeService _admin;
    private readonly WaoDbContext _db;

    public AdminFoodsController(IAdminCompositeService admin, WaoDbContext db)
    {
        _admin = admin;
        _db = db;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var total = await _db.FoodItems.CountAsync();
        var visible = await _db.FoodItems.CountAsync(f => f.Status == FoodStatus.Approved);
        var hidden = total - visible;
        var categories = await _db.FoodCategories.CountAsync();
        return Ok(new { success = true, data = new { total, visible, hidden, categories } });
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var cats = await _db.FoodCategories
            .Select(c => new
            {
                id = c.Id,
                name = c.NameVi,
                nameEn = c.NameEn,
                foodCount = _db.FoodItems.Count(f => f.CategoryId == c.Id)
            })
            .OrderBy(c => c.id)
            .ToListAsync();
        return Ok(new { success = true, data = cats });
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<FoodDetailResponse>>> Create([FromForm] CreateOfficialFoodRequest request)
    {
        Guid adminId = User.GetUserId();
        var result = await _admin.Foods.CreateOfficialFoodAsync(adminId, request);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<FoodDetailResponse>.Success(result, "Tạo món ăn thành công", "201"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<FoodDetailResponse>>> UpdateMetadata([FromRoute] Guid id, [FromBody] UpdateFoodMetadataRequest request)
    {
        Guid adminId = User.GetUserId();
        var result = await _admin.Foods.UpdateFoodMetadataAsync(adminId, id, request);
        return Ok(ApiResponse<FoodDetailResponse>.Success(result, "Cập nhật metadata thành công"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteFood([FromRoute] Guid id)
    {
        Guid adminId = User.GetUserId();
        await _admin.Foods.DeleteFoodAsync(adminId, id);
        return Ok(ApiResponse<object>.Success(null!, "Xóa món ăn thành công"));
    }

    [HttpPost("{id:guid}/nutrition")]
    public async Task<ActionResult<ApiResponse<FoodDetailResponse>>> AddOrUpdateNutrition([FromRoute] Guid id, [FromBody] CreateFoodNutritionDto request)
    {
        Guid adminId = User.GetUserId();
        var result = await _admin.Foods.AddOrUpdateNutritionAsync(adminId, id, request);
        return Ok(ApiResponse<FoodDetailResponse>.Success(result, "Cập nhật nutrition thành công"));
    }

    [HttpPost("{id:guid}/images")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<object>>> UploadImage([FromRoute] Guid id, IFormFile image)
    {
        Guid adminId = User.GetUserId();
        var result = await _admin.Foods.UploadFoodImageAsync(adminId, id, image);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<object>.Success(result, "Thêm ảnh thành công", "201"));
    }

    [HttpPut("{id:guid}/images/active")]
    public async Task<ActionResult<ApiResponse<FoodDetailResponse>>> SetActiveImage([FromRoute] Guid id, [FromBody] SetActiveImageRequest request)
    {
        Guid adminId = User.GetUserId();
        var result = await _admin.Foods.SetActiveImageAsync(adminId, id, request);
        return Ok(ApiResponse<FoodDetailResponse>.Success(result, "Đặt ảnh hiển thị thành công"));
    }

    [HttpDelete("{id:guid}/images/{imageId:long}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteImage([FromRoute] Guid id, [FromRoute] ulong imageId)
    {
        Guid adminId = User.GetUserId();
        await _admin.Foods.DeleteImageAsync(adminId, id, imageId);
        return Ok(ApiResponse<object>.Success(null!, "Xóa ảnh thành công"));
    }

    [HttpPost("{id:guid}/components")]
    public async Task<ActionResult<ApiResponse<FoodDetailResponse>>> AddComponent([FromRoute] Guid id, [FromBody] AddComponentRequest request)
    {
        Guid adminId = User.GetUserId();
        var result = await _admin.Foods.AddComponentAsync(adminId, id, request);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<FoodDetailResponse>.Success(result, "Thêm thành phần thành công", "201"));
    }

    [HttpDelete("{id:guid}/components/{componentId:long}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteComponent([FromRoute] Guid id, [FromRoute] ulong componentId)
    {
        Guid adminId = User.GetUserId();
        await _admin.Foods.DeleteComponentAsync(adminId, id, componentId);
        return Ok(ApiResponse<object>.Success(null!, "Xóa thành phần thành công"));
    }

    [HttpGet("pending")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<FoodSearchResponse>>>> GetPendingFoods([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _admin.Foods.GetPendingFoodsAsync(page, pageSize);
        return Ok(ApiResponse<PaginatedResponse<FoodSearchResponse>>.Success(result, "Lấy danh sách chờ duyệt thành công"));
    }

    [HttpPut("{id:guid}/review")]
    public async Task<ActionResult<ApiResponse<FoodDetailResponse>>> ReviewCommunityFood([FromRoute] Guid id, [FromBody] ReviewCommunityFoodRequest request)
    {
        Guid adminId = User.GetUserId();
        var result = await _admin.Foods.ReviewCommunityFoodAsync(adminId, id, request);
        return Ok(ApiResponse<FoodDetailResponse>.Success(result, "Duyệt món ăn thành công"));
    }
}
