# Skill: dotnet-backend

**Trigger:** Load this skill when the task involves EF Core migrations, JWT authentication, or xUnit testing.

> **Mobile context:** This backend serves an iOS + Android mobile app (Expo/React Native).
> Mobile sessions are long-lived (weeks), users may have multiple devices, and network
> conditions vary — keep responses lean and never block the thread.

---

## Skill 1 — EF Core Migrations

### When to create a migration
Create a migration whenever you add/remove/modify:
- A new `DbSet<T>` on `WaoDbContext`
- A property on an existing entity in `Models/`
- A relationship (navigation property / foreign key)

### Commands (run from the project root: `nutrition-app-backend/`)

```powershell
# Add a new migration
dotnet ef migrations add <MigrationName> --project nutrition-app-backend.csproj

# Apply pending migrations to the database
dotnet ef database update

# Roll back to a specific migration
dotnet ef database update <PreviousMigrationName>

# View pending migrations
dotnet ef migrations list
```

### EF Core patterns in this project

```csharp
// ✅ Always async
var user = await _dbContext.Users
    .AsNoTracking()                          // read-only
    .Include(u => u.UserProfile)             // eager load
    .FirstOrDefaultAsync(u => u.Id == id);

// ✅ Wrap multiple SaveChanges in a transaction
await using var tx = await _dbContext.Database.BeginTransactionAsync();
try
{
    _dbContext.Users.Add(newUser);
    await _dbContext.SaveChangesAsync();
    _dbContext.UserAuthProviders.Add(newAuth);
    await _dbContext.SaveChangesAsync();
    await tx.CommitAsync();
}
catch
{
    await tx.RollbackAsync();
    throw;
}
```

### Entity conventions used in this project
- Primary keys: `Guid Id` with `default = Guid.NewGuid()`
- Timestamps: `DateTime CreatedAt` (UTC)
- Soft delete: not currently implemented — use hard delete
- Nullable reference types are **enabled** — mark optional properties with `?`

---

## Skill 2 — JWT Authentication

### How JWT works in this project
1. Client sends Google ID Token → `POST /api/auth/google`
2. `AuthService.LoginWithGoogleAsync` validates the token with Google's library
3. `TokenService.CreateTokensAsync` generates a JWT access token + refresh token
4. Client stores and sends: `Authorization: Bearer <accessToken>`

### Reading the current user in a controller

```csharp
// Extension method already exists in Extensions/
Guid userId = User.GetUserId(); // ClaimsPrincipal extension
```

### Protecting endpoints

```csharp
[Authorize]                    // Requires valid JWT
[Authorize(Roles = "Admin")]   // Requires role claim
[AllowAnonymous]               // Explicitly public
```

### Adding claims to JWT (in TokenService)

```csharp
var claims = new List<Claim>
{
    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new(ClaimTypes.Email, email),
    // Add custom claims here, e.g.:
    new("is_new_user", isNewUser.ToString().ToLower())
};
```

### JWT config lives in `appsettings.Development.json`

```json
{
  "Jwt": {
    "Key": "...",
    "Issuer": "...",
    "Audience": "...",
    "AccessTokenExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 30
  }
}
```

### Mobile-specific token guidance

**Access token lifetime:** 60 minutes is fine. Mobile apps should silently refresh using the refresh token.

**Refresh token lifetime:** 30 days minimum for mobile (users shouldn't be logged out after a week).
Can extend to 90 days for "Remember Me" flows.

**Refresh token rotation:** When a refresh token is used, issue a **new** refresh token and invalidate the old one.
This detects stolen tokens — if the old token is replayed, both old and new should be revoked.

```csharp
// ✅ Rotation pattern in TokenService.RefreshAsync
public async Task<AuthResponse?> RefreshAsync(string oldToken)
{
    var existing = await _dbContext.RefreshTokens
        .FirstOrDefaultAsync(t => t.Token == oldToken && !t.IsRevoked);

    if (existing == null || existing.ExpiresAt < DateTime.UtcNow)
        return null;

    // Revoke old token
    existing.IsRevoked = true;

    // Issue new refresh token
    var newRefresh = new RefreshToken { UserId = existing.UserId, ... };
    _dbContext.RefreshTokens.Add(newRefresh);
    await _dbContext.SaveChangesAsync();

    return await CreateTokensAsync(existing.User, isNewUser: false, email: ...);
}
```

**Multi-device logout:** On `POST /api/auth/logout`, revoke only the token for the current device.
To log out all devices, add a `POST /api/auth/logout-all` that revokes all tokens for the user.

```csharp
// Logout current device
public async Task RevokeAsync(string refreshToken)
    => await _dbContext.RefreshTokens
        .Where(t => t.Token == refreshToken)
        .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsRevoked, true));

// Logout all devices
public async Task RevokeAllAsync(Guid userId)
    => await _dbContext.RefreshTokens
        .Where(t => t.UserId == userId && !t.IsRevoked)
        .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsRevoked, true));
```

**iOS/Android client IDs:** Already handled in `AuthService` — the Google validation settings
accept `WebClientId`, `IosClientId`, and `AndroidClientId` from config. Do not remove any.

---

## Skill 3 — xUnit Testing Patterns

### Test project setup
Create a separate test project (if it doesn't exist):

```powershell
dotnet new xunit -n nutrition-app-backend.Tests
dotnet add nutrition-app-backend.Tests/nutrition-app-backend.Tests.csproj reference nutrition-app-backend/nutrition-app-backend.csproj
dotnet add nutrition-app-backend.Tests package Moq
dotnet add nutrition-app-backend.Tests package Microsoft.EntityFrameworkCore.InMemory
```

### Service unit test pattern (using Moq)

```csharp
public class UserServiceTests
{
    private readonly Mock<WaoDbContext> _dbMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _sut = new UserService(_dbMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetUserInfoAsync_ReturnsNull_WhenUserNotFound()
    {
        // Arrange
        _dbMock.Setup(db => db.Users.FindAsync(It.IsAny<Guid>()))
               .ReturnsAsync((User?)null);

        // Act
        var result = await _sut.GetUserInfoAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }
}
```

### In-memory DB integration test pattern

```csharp
public class AuthServiceIntegrationTests
{
    private WaoDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<WaoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new WaoDbContext(options);
    }

    [Fact]
    public async Task LoginWithGoogle_CreatesNewUser_WhenProviderNotFound()
    {
        // Arrange
        using var db = CreateInMemoryDb();
        // ... setup mocks for IConfiguration and ITokenService
        var service = new AuthService(db, configMock.Object, tokenMock.Object);

        // Act & Assert
        // ...
    }
}
```

### Controller test pattern (using WebApplicationFactory)

```csharp
public class UserControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UserControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace real DB with in-memory
                services.AddDbContext<WaoDbContext>(options =>
                    options.UseInMemoryDatabase("test"));
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetUserInfo_Returns401_WhenNoToken()
    {
        var response = await _client.GetAsync("/api/user/info");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
```

### Naming convention for tests
`MethodName_ExpectedBehavior_WhenCondition`

```
GetUserInfoAsync_ReturnsNull_WhenUserNotFound
LoginWithGoogle_CreatesNewUser_WhenProviderNotFound
Refresh_Returns401_WhenTokenExpired
```
