# Skill: api-testing

**Trigger:** Load this skill when the task involves generating REST API tests, running endpoint validation, or creating `.http` test files.

---

## Overview

This project already has a `nutrition-app-backend.http` file for manual HTTP testing via JetBrains HTTP Client or VS Code REST Client.

---

## Skill 1 — `.http` File Tests (Manual / Quick Validation)

Location: `nutrition-app-backend/nutrition-app-backend.http`

### Template for a new endpoint test block

```http
### [Feature] — [Endpoint Description]
# Variables set above or in http-client.env.json

POST {{baseUrl}}/api/auth/google
Content-Type: application/json

{
  "idToken": "{{googleIdToken}}"
}

###

### Refresh Token
POST {{baseUrl}}/api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "{{refreshToken}}"
}

###

### Get User Info (Authenticated)
GET {{baseUrl}}/api/user/info
Authorization: Bearer {{accessToken}}

###
```

### Environment file (`http-client.env.json`) — gitignored

```json
{
  "dev": {
    "baseUrl": "http://localhost:5000",
    "accessToken": "",
    "refreshToken": "",
    "googleIdToken": ""
  }
}
```

---

## Skill 2 — Automated Integration Tests with xUnit + HttpClient

### Full auth flow test

```csharp
[Fact]
public async Task FullAuthFlow_GoogleLogin_ThenGetProfile_Succeeds()
{
    // Step 1: Login (mock Google token validation for test environment)
    var loginResponse = await _client.PostAsJsonAsync("/api/auth/google", new
    {
        idToken = "test-google-id-token"
    });
    loginResponse.EnsureSuccessStatusCode();

    var loginBody = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
    var accessToken = loginBody!.Data!.AccessToken;

    // Step 2: Use token to get user info
    _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", accessToken);

    var infoResponse = await _client.GetAsync("/api/user/info");
    Assert.Equal(HttpStatusCode.OK, infoResponse.StatusCode);
}
```

### Running tests

```powershell
# Run all tests
dotnet test

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"

# Run a specific test class
dotnet test --filter "FullClassName=nutrition_app_backend.Tests.UserControllerTests"

# Run with coverage (requires coverlet)
dotnet test --collect:"XPlat Code Coverage"
```

---

## Skill 3 — API Response Validation Helpers

Add this to your test project for typed assertion helpers:

```csharp
public static class ApiAssert
{
    public static async Task<ApiResponse<T>> IsSuccess<T>(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        Assert.NotNull(body);
        Assert.True(body.IsSuccess, $"Expected success but got: {body.Message}");
        return body;
    }

    public static async Task<ApiResponse<object>> IsFail(
        HttpResponseMessage response,
        HttpStatusCode expectedStatus)
    {
        Assert.Equal(expectedStatus, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(body);
        Assert.False(body!.IsSuccess);
        return body;
    }
}
```

### Usage

```csharp
var response = await _client.GetAsync("/api/user/info");
var body = await ApiAssert.IsSuccess<GetUserInfoResponse>(response);
Assert.NotNull(body.Data);
```

---

## Skill 4 — Test Checklist for New Endpoints

When a new endpoint is created, generate tests for:

- [ ] **Happy path** — valid input, correct response shape
- [ ] **Unauthorized** — no token → 401
- [ ] **Not found** — entity doesn't exist → 404 with `ApiResponse.Fail`
- [ ] **Bad input** — missing/invalid body → 400
- [ ] **Idempotency** — PUT/DELETE called twice returns consistent result

---

## Skill 5 — DB Health Check (Already Implemented)

```http
### Health Check
GET http://localhost:5000/api/health/db
```

Expected response: `"Database connection successful!"`
