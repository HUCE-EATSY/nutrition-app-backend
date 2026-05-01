# Workflow: /api-new

**Command:** `/api-new`  
**Purpose:** Scaffold a complete new API feature — Controller, Service interface + implementation, and DTOs — in one go.

> **Mobile backend:** All scaffolded routes use `api/v1/` prefix and all list endpoints
> are paginated with `PagedResponse<T>` by default.

---

## Trigger

Use this workflow when asked to add a **new domain endpoint** (e.g., "add a Meals API", "create a NutritionLog feature").

---

## Step-by-Step Execution Plan

Before writing any code, state this plan and confirm with the user if the feature name is ambiguous.

```
0. Decide: record vs class for each DTO (see decision table below)
1. Identify feature name (e.g., "Meal")
2. Create DTOs → verify: files exist in DTOs/{Feature}/
3. Create Service interface + implementation → verify: files exist in Services/{Feature}/
4. Register service in Program.cs → verify: AddScoped<> line added
5. Create Controller → verify: file exists in Controllers/
6. (Optional) Create EF Entity + add to DbContext → verify: migration needed?
```

---

## Step 0: Decide — `record` vs `class` for DTOs

Apply this decision **before writing any DTO**. The wrong choice here is hard to refactor later.

### Decision Table

| Situation | Use |
|-----------|-----|
| **Response DTO** — data flowing *out* to the mobile client | ✅ `record` |
| **Simple request DTO** — just flat properties, no validation attributes | ✅ `record` |
| **Request DTO with `[Required]`, `[Range]`, `[MaxLength]`** etc. | ⚠️ `class` |
| **Request DTO with nested mutable objects** (e.g., `List<ItemRequest>`) | ⚠️ `class` |
| **DTO that AutoMapper maps *into* using `ForMember`** | ⚠️ `class` |
| **Paginated wrapper** (`PagedResponse<T>`, `PagedRequest`) | ✅ `class` (has computed property + mutable list) |

### Why this rule?

- `record` gives you **immutability** (`init`-only setters), **value equality**, and terser syntax — ideal for response payloads that are created once and sent.
- `class` is needed when ASP.NET model binding or AutoMapper needs to **set properties after construction**, or when DataAnnotation validators are involved.
- Both serialize identically with `System.Text.Json` — the mobile client sees no difference.

### Syntax reference

```csharp
// ✅ record — Response DTO (immutable, value equality)
public record MealResponse(
    Guid Id,
    string Name,
    int CaloriesKcal,
    DateTime CreatedAt
);

// ✅ record — Simple request DTO (no validation attributes)
public record CreateMealRequest(
    string Name,
    int CaloriesKcal
);

// ⚠️ class — Request DTO with validation attributes
public class UpdateProfileRequest
{
    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [Range(1, 300)]
    public int? HeightCm { get; set; }
}

// ⚠️ class — Request DTO with nested list
public class LogMealRequest
{
    public DateTime LoggedAt { get; set; }
    public List<MealItemRequest> Items { get; set; } = [];
}
```

### AutoMapper note

AutoMapper 12 (used in this project) supports both `record` and `class`. However, when mapping **into** a `record`, AutoMapper uses the constructor — so the constructor parameter names must match the source property names exactly (case-insensitive).

```csharp
// ✅ Works — constructor params match source properties
public record MealResponse(Guid Id, string Name);
CreateMap<Meal, MealResponse>(); // no ForMember needed

// ❌ Fails — ForMember cannot target init-only record properties
CreateMap<Meal, MealResponse>()
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title)); // use class instead
```

---

## File Templates

Replace `{Feature}` with the actual feature name (e.g., `Meal`, `NutritionLog`).

---

### Step 1: DTOs — `DTOs/{Feature}/`

**Apply Step 0 first** to decide `record` vs `class` before writing these files.

**`Create{Feature}Request.cs`** — Input DTO

```csharp
namespace nutrition_app_backend.DTOs.{Feature};

// ✅ Use record if no validation attributes and no nested mutable lists
public record Create{Feature}Request(
    string Name
    // TODO: add fields specific to this feature
);

// ⚠️ Use class instead if you need [Required], [Range], etc.
// public class Create{Feature}Request
// {
//     [Required] [MaxLength(100)]
//     public string Name { get; set; } = string.Empty;
// }
```

**`{Feature}Response.cs`** — Output DTO

```csharp
namespace nutrition_app_backend.DTOs.{Feature};

// ✅ Always record for responses — immutable, concise, value equality
public record {Feature}Response(
    Guid Id,
    string Name,
    DateTime CreatedAt
    // TODO: add fields specific to this feature
);
```

---

### Step 2: Service Interface — `Services/{Feature}/I{Feature}Service.cs`

```csharp
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.{Feature};

namespace nutrition_app_backend.Services.{Feature};

public interface I{Feature}Service
{
    Task<{Feature}Response?> Create{Feature}Async(Guid userId, Create{Feature}Request request);
    Task<{Feature}Response?> Get{Feature}Async(Guid userId, Guid id);
    Task<PagedResponse<{Feature}Response>> GetAll{Feature}sAsync(Guid userId, PagedRequest paging);
    Task<bool> Delete{Feature}Async(Guid userId, Guid id);
}
```

---

### Step 3: Service Implementation — `Services/{Feature}/{Feature}Service.cs`

```csharp
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.{Feature};

namespace nutrition_app_backend.Services.{Feature};

public class {Feature}Service : I{Feature}Service
{
    private readonly WaoDbContext _dbContext;
    private readonly IMapper _mapper;

    public {Feature}Service(WaoDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<{Feature}Response?> Create{Feature}Async(Guid userId, Create{Feature}Request request)
    {
        // TODO: map request → entity, add to DbContext, save
        throw new NotImplementedException();
    }

    public async Task<{Feature}Response?> Get{Feature}Async(Guid userId, Guid id)
    {
        // TODO: query with AsNoTracking, return mapped DTO or null
        throw new NotImplementedException();
    }

    public async Task<PagedResponse<{Feature}Response>> GetAll{Feature}sAsync(Guid userId, PagedRequest paging)
    {
        var query = _dbContext.{Feature}s
            .Where(x => x.UserId == userId)
            .AsNoTracking();

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(Math.Min(paging.PageSize, 50))
            .ToListAsync();

        return new PagedResponse<{Feature}Response>
        {
            Items = _mapper.Map<List<{Feature}Response>>(items),
            Page = paging.Page,
            PageSize = paging.PageSize,
            TotalCount = total
        };
    }

    public async Task<bool> Delete{Feature}Async(Guid userId, Guid id)
    {
        // TODO: find entity, remove, save
        throw new NotImplementedException();
    }
}
```

---

### Step 4: Register in `Program.cs`

Add this line in the `// SERVICES / INTERFACES` section:

```csharp
builder.Services.AddScoped<I{Feature}Service, {Feature}Service>();
```

---

### Step 5: Controller — `Controllers/{Feature}Controller.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.{Feature};
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.{Feature};

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]   // versioned for mobile
[Authorize]
public class {Feature}Controller : ControllerBase
{
    private readonly I{Feature}Service _{feature}Service;

    public {Feature}Controller(I{Feature}Service {feature}Service)
    {
        _{feature}Service = {feature}Service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Create{Feature}Request request)
    {
        Guid userId = User.GetUserId();
        var result = await _{feature}Service.Create{Feature}Async(userId, request);
        if (result == null)
            return BadRequest(ApiResponse<object>.Fail("Không thể tạo {feature}.", "400"));
        return Ok(ApiResponse<{Feature}Response>.Success(result, "Tạo thành công"));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        Guid userId = User.GetUserId();
        var result = await _{feature}Service.Get{Feature}Async(userId, id);
        if (result == null)
            return NotFound(ApiResponse<object>.Fail("Không tìm thấy {feature}.", "404"));
        return Ok(ApiResponse<{Feature}Response>.Success(result, "Lấy thành công"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest paging)
    {
        Guid userId = User.GetUserId();
        var result = await _{feature}Service.GetAll{Feature}sAsync(userId, paging);
        return Ok(ApiResponse<PagedResponse<{Feature}Response>>.Success(result, "Lấy danh sách thành công"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        Guid userId = User.GetUserId();
        var deleted = await _{feature}Service.Delete{Feature}Async(userId, id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail("Không tìm thấy {feature}.", "404"));
        return Ok(ApiResponse<object>.Success(null!, "Xóa thành công"));
    }
}
```

---

## Verification Checklist

After scaffolding, confirm:

- [ ] **Step 0 decision documented** — stated why each DTO is `record` or `class`
- [ ] `DTOs/{Feature}/Create{Feature}Request.cs` exists (`record` or `class` per Step 0)
- [ ] `DTOs/{Feature}/{Feature}Response.cs` exists (should be `record`)
- [ ] `Services/{Feature}/I{Feature}Service.cs` exists
- [ ] `Services/{Feature}/{Feature}Service.cs` exists
- [ ] `Program.cs` has `AddScoped<I{Feature}Service, {Feature}Service>()`
- [ ] `Controllers/{Feature}Controller.cs` exists
- [ ] AutoMapper profile added in `Mappings/` if `ForMember` is needed
- [ ] No `NotImplementedException` left in final code
- [ ] Build passes: `dotnet build`
