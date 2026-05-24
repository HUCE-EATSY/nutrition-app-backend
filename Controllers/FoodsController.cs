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
    /// Get a paginated list of foods (for default display, no search).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<FoodSearchResponse>>>> GetList([FromQuery] FoodListRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _foodService.GetListAsync(request, userId);

        return Ok(ApiResponse<PaginatedResponse<FoodSearchResponse>>.Success(result, "Lấy danh sách thành công"));
    }

    /// <summary>
    /// Fulltext search food items. Approved items visible to all; pending items only to creator.
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<FoodSearchResponse>>>> Search([FromQuery] FoodSearchRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _foodService.SearchAsync(request, userId);

        return Ok(ApiResponse<PaginatedResponse<FoodSearchResponse>>.Success(result, "Tìm kiếm thành công"));
    }

    /// <summary>
    /// Get food detail by ID with nutrition and image.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<FoodDetailResponse>>> GetById([FromRoute] Guid id)
    {
        var result = await _foodService.GetByIdAsync(id);

        return Ok(ApiResponse<FoodDetailResponse>.Success(result, "Lấy chi tiết món ăn thành công"));
    }

    /// <summary>
    /// Get components (children) of a food item.
    /// </summary>
    [HttpGet("{id:guid}/components")]
    public async Task<ActionResult<ApiResponse<List<FoodComponentResponse>>>> GetComponents([FromRoute] Guid id)
    {
        var result = await _foodService.GetComponentsAsync(id);

        return Ok(ApiResponse<List<FoodComponentResponse>>.Success(result, "Lấy thành phần thành công"));
    }

    /// <summary>
    /// Barcode lookup. Returns 404 if not found.
    /// </summary>
    [HttpGet("barcode/{barcode:long}")]
    public async Task<ActionResult<ApiResponse<FoodDetailResponse>>> GetByBarcode([FromRoute] ulong barcode)
    {
        var result = await _foodService.GetByBarcodeAsync(barcode);

        return Ok(ApiResponse<FoodDetailResponse>.Success(result, "Tìm thấy sản phẩm"));
    }

    /// <summary>
    /// Create a community food item (source=3, status=0 pending).
    /// Gửi dưới dạng multipart/form-data. Ảnh (image) sẽ được backend upload lên Cloudinary.
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<FoodDetailResponse>>> Create([FromForm] CreateFoodRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _foodService.CreateAsync(request, userId);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<FoodDetailResponse>.Success(result, "Tạo món ăn thành công", "201"));
    }

    /// <summary>
    /// Create a custom recipe (composite food) from existing ingredients.
    /// Gửi dưới dạng multipart/form-data.
    /// </summary>
    [HttpPost("recipes")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<FoodDetailResponse>>> CreateRecipe([FromForm] CreateRecipeRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _foodService.CreateRecipeAsync(request, userId);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<FoodDetailResponse>.Success(result, "Tạo công thức thành công", "201"));
    }
}
