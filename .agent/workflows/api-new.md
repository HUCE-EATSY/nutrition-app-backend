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
1. Identify feature name (e.g., "Meal")
2. Create DTOs → verify: files exist in DTOs/{Feature}/
3. Create Service interface + implementation → verify: files exist in Services/{Feature}/
4. Register service in Program.cs → verify: AddScoped<> line added
5. Create Controller → verify: file exists in Controllers/
6. (Optional) Create EF Entity + add to DbContext → verify: migration needed?
```

---

## File Templates

Replace `{Feature}` with the actual feature name (e.g., `Meal`, `NutritionLog`).

---

### Step 1: DTOs — `DTOs/{Feature}/`

**`{Feature}Request.cs`** — Input DTO

```csharp
namespace nutrition_app_backend.DTOs.{Feature};

public class Create{Feature}Request
{
    public string Name { get; set; } = string.Empty;
    // TODO: add fields specific to this feature
}
```

**`{Feature}Response.cs`** — Output DTO

```csharp
namespace nutrition_app_backend.DTOs.{Feature};

public class {Feature}Response
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    // TODO: add fields specific to this feature
}
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

- [ ] `DTOs/{Feature}/Create{Feature}Request.cs` exists
- [ ] `DTOs/{Feature}/{Feature}Response.cs` exists
- [ ] `Services/{Feature}/I{Feature}Service.cs` exists
- [ ] `Services/{Feature}/{Feature}Service.cs` exists
- [ ] `Program.cs` has `AddScoped<I{Feature}Service, {Feature}Service>()`
- [ ] `Controllers/{Feature}Controller.cs` exists
- [ ] No `[NotImplementedException]` left in final code
- [ ] Build passes: `dotnet build`
