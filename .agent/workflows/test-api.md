# Workflow: /test-api

**Command:** `/test-api`  
**Purpose:** Build the project, apply pending EF Core migrations, and execute integration tests end-to-end.

---

## Trigger

Use this workflow when asked to:
- Verify the project compiles after changes
- Apply new migrations before testing
- Run the full test suite

---

## Step-by-Step Execution Plan

State this plan before running any commands:

```
1. Build project → verify: 0 errors
2. Check pending migrations → verify: list pending migrations
3. Apply migrations → verify: database updated
4. Run tests → verify: all tests pass (0 failed)
5. Report summary
```

---

## Step 1: Build the Project

```powershell
# Run from solution root or project folder
dotnet build nutrition-app-backend/nutrition-app-backend.csproj

# ✅ Success indicator: "Build succeeded."
# ❌ Failure: Fix all errors before proceeding — do NOT run migrations on a broken build
```

---

## Step 2: Check Pending Migrations

```powershell
dotnet ef migrations list --project nutrition-app-backend/nutrition-app-backend.csproj

# Output shows all migrations with [*] marking ones NOT yet applied to the DB
# Example:
#   20240101_InitialCreate
#   20240215_AddUserProfile [*]  ← pending
```

If there are **no pending migrations**, skip Step 3.

---

## Step 3: Apply Migrations

```powershell
dotnet ef database update --project nutrition-app-backend/nutrition-app-backend.csproj

# ✅ Success: "Done." with list of applied migrations
# ❌ If connection fails: check appsettings.Development.json ConnectionStrings:DefaultConnection
```

### Rollback (if needed)

```powershell
# Roll back to a specific migration (use name from `migrations list`)
dotnet ef database update <MigrationName> --project nutrition-app-backend/nutrition-app-backend.csproj
```

---

## Step 4: Run Integration Tests

```powershell
# Run all tests (if test project exists)
dotnet test nutrition-app-backend.Tests/nutrition-app-backend.Tests.csproj --logger "console;verbosity=normal"

# Run only a specific category
dotnet test --filter "Category=Integration"

# Run with coverage report
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
```

### Expected output

```
Test Run Successful.
Total tests: X
     Passed: X
     Failed: 0
```

---

## Step 5: Manual Smoke Test (`.http` file)

After automated tests pass, manually verify the live server with the HTTP client:

```powershell
# Start the dev server
dotnet run --project nutrition-app-backend/nutrition-app-backend.csproj
```

Then run these requests from `nutrition-app-backend.http`:

1. `GET /api/health/db` → `"Database connection successful!"`
2. `POST /api/auth/google` → `{ isSuccess: true, data: { accessToken: "..." } }`
3. `GET /api/user/info` (with Bearer token) → `{ isSuccess: true, data: { ... } }`

---

## Step 6: Report Summary

After all steps complete, output a table:

| Step              | Status | Notes                          |
|-------------------|--------|--------------------------------|
| Build             | ✅/❌   | Error count if failed          |
| Migrations        | ✅/❌   | Migration names applied        |
| Tests             | ✅/❌   | Pass/fail counts               |
| Manual smoke test | ✅/❌   | Any unexpected responses       |

If any step fails, **stop and report** — do not continue to the next step.

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| `No connection string` | Check `appsettings.Development.json` → `ConnectionStrings:DefaultConnection` |
| `Migration already applied` | Run `dotnet ef migrations list` to see current state |
| `Test project not found` | Create with `dotnet new xunit -n nutrition-app-backend.Tests` |
| `401 on smoke test` | Token may be expired — get a fresh Google ID token |
| `Port in use` | Kill process on port 5000 or use `--urls http://localhost:5001` |
