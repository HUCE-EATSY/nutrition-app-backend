using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.Food;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/foods")]
[Authorize]
public class FoodsController : ControllerBase
{
    private readonly IFoodService _foodService;

    public FoodsController(IFoodService foodService)
    {
        _foodService = foodService;
    }

    /// <summary>
    /// Fulltext search food items. Approved items visible to all; pending items only to creator.
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] FoodSearchRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _foodService.SearchAsync(request, userId);

        return Ok(ApiResponse<PaginatedResponse<FoodSearchResponse>>.Success(result, "Tìm kiếm thành công"));
    }

    /// <summary>
    /// Get food detail by ID with nutrition and image.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _foodService.GetByIdAsync(id);

        return Ok(ApiResponse<FoodDetailResponse>.Success(result, "Lấy chi tiết món ăn thành công"));
    }

    /// <summary>
    /// Get components (children) of a food item.
    /// </summary>
    [HttpGet("{id:guid}/components")]
    public async Task<IActionResult> GetComponents(Guid id)
    {
        var result = await _foodService.GetComponentsAsync(id);

        return Ok(ApiResponse<List<FoodComponentResponse>>.Success(result, "Lấy thành phần thành công"));
    }

    /// <summary>
    /// Barcode lookup. Returns 404 if not found.
    /// </summary>
    [HttpGet("barcode/{barcode:long}")]
    public async Task<IActionResult> GetByBarcode(ulong barcode)
    {
        var result = await _foodService.GetByBarcodeAsync(barcode);

        return Ok(ApiResponse<FoodDetailResponse>.Success(result, "Tìm thấy sản phẩm"));
    }

    /// <summary>
    /// Create a community food item (source=3, status=0 pending).
    /// Client must upload image to storage first and send thumbnail_url.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFoodRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _foodService.CreateAsync(request, userId);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<FoodDetailResponse>.Success(result, "Tạo món ăn thành công", "201"));
    }
}
