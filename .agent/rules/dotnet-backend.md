# Rule: dotnet-backend — ASP.NET 8 Coding Standards

These rules are **mandatory guardrails** for every coding task in this repository.
Violating any rule is a hard error — surface it before writing code.

> **Context:** This backend serves a **mobile application** (iOS + Android via Expo/React Native).
> Native mobile clients have no CORS restrictions, use long-lived sessions, and rely on
> API versioning since app updates cannot be forced. Design every endpoint with this in mind.

---

## 1. API Style: Minimal APIs vs Controllers

This project uses **Controller-based APIs** (`[ApiController]`, `ControllerBase`).

- **Prefer Minimal APIs** (`app.MapGet`, `app.MapPost`, etc.) **only** for:
  - Simple one-off utility endpoints (e.g., the existing `/api/health/db`).
  - Endpoints with no business logic or shared dependencies.
- **Use Controller classes** for all domain endpoints (Auth, User, Nutrition, etc.)  
  because they benefit from route grouping, `[Authorize]`, and `[FromBody]` conventions already in place.
- ❌ **Never** mix styles for the same domain (e.g., don't add a Minimal API for `/api/auth/*`).

---

## 2. Async/Await — Non-Negotiable

- **Every** method that touches I/O (database, HTTP, file) **must** be `async Task<T>`.
- Method names must end in `Async` (e.g., `GetUserInfoAsync`).
- ❌ **Forbidden**: `.Result`, `.Wait()`, `.GetAwaiter().GetResult()` — these cause deadlocks.
- ❌ **Forbidden**: `async void` (except for event handlers, which don't exist here).

```csharp
// ✅ Correct
public async Task<UserProfileResponse?> GetProfileAsync(Guid userId)
    => await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

// ❌ Wrong — blocks the thread
public UserProfileResponse? GetProfile(Guid userId)
    => _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId).Result;
```

---

## 3. DTOs — Never Expose EF Entities Directly

- Controllers **must** accept and return DTOs, never raw EF entity classes from `Models/`.
- DTO folders live in `DTOs/{Domain}/` (e.g., `DTOs/Users/`, `DTOs/Auth/`).
- Use **AutoMapper** (already configured) to map between entities and DTOs.
- Standard response shape is `ApiResponse<T>` from `DTOs/ApiResponse.cs`.

| Layer        | Type used              |
|--------------|------------------------|
| Controller   | Request/Response DTOs  |
| Service      | DTOs (input → return)  |
| Repository   | EF Entity Models       |
| API Response | `ApiResponse<DTO>`     |

```csharp
// ✅ Correct — returns a DTO
return Ok(ApiResponse<UserProfileResponse>.Success(result, "OK"));

// ❌ Wrong — exposes EF entity
return Ok(user); // User is an EF Model
```

---

## 4. Dependency Injection

- **All** services must be registered in `Program.cs` via the built-in DI container.
- Use the appropriate lifetime:
  - `AddScoped<>` — per HTTP request (default for services using `DbContext`).
  - `AddSingleton<>` — stateless utilities (e.g., config readers).
  - `AddTransient<>` — lightweight, stateless helpers.
- ❌ **Never** use `new MyService()` inside controllers or other services — always inject.
- ❌ **Never** use `ServiceLocator` or `IServiceProvider` manually inside business logic.

```csharp
// ✅ Correct — registered in Program.cs
builder.Services.AddScoped<IUserService, UserService>();

// ✅ Correct — injected via constructor
public UserController(IUserService userService) { _userService = userService; }

// ❌ Wrong — service locator anti-pattern
var svc = app.Services.GetRequiredService<IUserService>();
```

---

## 5. EF Core — Database Rules

- Always use `async` EF methods: `FirstOrDefaultAsync`, `ToListAsync`, `SaveChangesAsync`.
- Never call `SaveChangesAsync` more than once per logical operation without a transaction.
- Prefer `Include()` for eager loading over lazy loading (lazy loading is not configured).
- Use `AsNoTracking()` for read-only queries to improve performance.

```csharp
// ✅ Read-only query
var user = await _dbContext.Users
    .AsNoTracking()
    .FirstOrDefaultAsync(u => u.Id == userId);
```

---

## 6. Error Handling & Responses

- Always return `ApiResponse<T>` — never return raw strings or anonymous objects.
- Map HTTP status codes correctly:
  - `200 Ok` — success with data
  - `401 Unauthorized` — missing/invalid token
  - `404 NotFound` — entity does not exist
  - `400 BadRequest` — validation failure
- ❌ **Never** catch `Exception` and swallow it silently — at minimum, log it.
- Mobile clients display these messages to users — write `Message` strings clearly (already using Vietnamese).

---

## 9. API Versioning — Required for Mobile

Mobile apps **cannot be force-updated**. An old version of the app must still work even after the API changes.

- **Always** prefix routes with a version segment: `/api/v1/user/...`
- Use URL-based versioning (simplest for mobile HTTP clients).
- When introducing breaking changes, create a `v2` route — do **not** modify `v1` in place.
- ❌ **Never** rename or remove a field from an existing response DTO without a new version.

```csharp
// ✅ Versioned route
[Route("api/v1/[controller]")]
public class UserController : ControllerBase { ... }

// ❌ Unversioned — breaks old app installs silently
[Route("api/[controller]")]
```

> **Note:** The current project uses unversioned routes (`api/[controller]`).  
> When adding **new** controllers, use `api/v1/[controller]`.  
> Migrate existing routes to `v1` only when deliberately versioning.

---

## 10. Pagination — Required for List Endpoints

Mobile UIs use **infinite scroll** — never return unbounded lists.

- Every endpoint that returns a list **must** be paginated.
- Use **cursor-based pagination** (preferred) or **page/size** for simpler cases.
- The response must include metadata so the client knows if there is a next page.

```csharp
// ✅ Standard paginated response DTO
public class PagedResponse<T>
{
    public List<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasNextPage => Page * PageSize < TotalCount;
}

// ✅ Standard paginated request DTO
public class PagedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20; // default, cap at 50
}

// ✅ Usage in service
public async Task<PagedResponse<MealResponse>> GetMealsAsync(Guid userId, PagedRequest req)
{
    var query = _dbContext.Meals.Where(m => m.UserId == userId).AsNoTracking();
    var total = await query.CountAsync();
    var items = await query
        .OrderByDescending(m => m.CreatedAt)  // always order for consistent paging
        .Skip((req.Page - 1) * req.PageSize)
        .Take(Math.Min(req.PageSize, 50))     // cap page size
        .ToListAsync();
    return new PagedResponse<MealResponse>
    {
        Items = _mapper.Map<List<MealResponse>>(items),
        Page = req.Page,
        PageSize = req.PageSize,
        TotalCount = total
    };
}
```

- ❌ **Never** do `ToListAsync()` on a table without `Take()` for list endpoints.

---

## 11. File & Image Uploads — Mobile Patterns

Mobile users upload profile photos, food images, etc. via `multipart/form-data`.

- Accept uploads via `IFormFile` — do **not** accept Base64-encoded strings in JSON (too large).
- Validate file type (`image/jpeg`, `image/png`, `image/webp`) and size (max 5 MB) before processing.
- Store files using the configured storage service — do **not** write directly to `wwwroot`.
- Return the **public URL** of the stored file in the response DTO, not binary content.

```csharp
// ✅ File upload endpoint pattern
[HttpPost("avatar")]
[Consumes("multipart/form-data")]
public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarRequest request)
{
    if (request.File.Length > 5 * 1024 * 1024)
        return BadRequest(ApiResponse<object>.Fail("File không được vượt quá 5MB.", "400"));

    var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
    if (!allowed.Contains(request.File.ContentType))
        return BadRequest(ApiResponse<object>.Fail("Định dạng file không hợp lệ.", "400"));

    // delegate to service
    var url = await _userService.UploadAvatarAsync(User.GetUserId(), request.File);
    return Ok(ApiResponse<string>.Success(url, "Tải ảnh lên thành công"));
}
```

---

## 12. Push Notifications — Device Token Storage

Mobile apps receive push notifications via FCM (Android) or APNs (iOS).

- Store device tokens in the DB linked to a `UserId` — a user can have multiple devices.
- Invalidate tokens when the user logs out or the token refresh fails.
- The push notification sending logic belongs in a dedicated `INotificationService`.
- ❌ **Never** call FCM/APNs directly from a controller.

```csharp
// ✅ Device token registration endpoint
[HttpPost("device-token")]
public async Task<IActionResult> RegisterDeviceToken([FromBody] DeviceTokenRequest request)
{
    Guid userId = User.GetUserId();
    await _notificationService.RegisterTokenAsync(userId, request.Token, request.Platform);
    return Ok(ApiResponse<object>.Success(null!, "Đăng ký thiết bị thành công"));
}
// Platform: "ios" | "android"
```

---

## 7. Naming Conventions

| Element       | Convention              | Example                    |
|---------------|-------------------------|----------------------------|
| Interface     | `I` prefix              | `IUserService`             |
| Service class | Plain name              | `UserService`              |
| DTO (request) | `...Request`            | `OnboardingRequest`        |
| DTO (response)| `...Response`           | `UserProfileResponse`      |
| Async method  | `...Async` suffix       | `GetUserInfoAsync`         |
| Controller    | `...Controller`         | `UserController`           |
| EF Entity     | Plain noun (no suffix)  | `User`, `UserGoal`         |

---

## 8. Project Structure (Do Not Deviate)

```
Controllers/        ← API entry points only, no business logic
Services/{Domain}/  ← IMyService.cs + MyService.cs
DTOs/{Domain}/      ← Request + Response DTOs
Models/Users/       ← EF Core entity classes
Data/               ← DbContext
Mappings/           ← AutoMapper profiles
Extensions/         ← ClaimsPrincipal helpers, etc.
Migrations/         ← EF migrations (generated, do not edit manually)
```

When adding a new feature, create files in **all relevant folders** before writing logic.
