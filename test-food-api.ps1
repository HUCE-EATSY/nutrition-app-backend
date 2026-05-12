# Test Food API Script
$baseUrl = "http://localhost:5184/api/v1"

Write-Host "=== Testing Food API ===" -ForegroundColor Green

# 1. Create Food
Write-Host "`n1. Creating new food..." -ForegroundColor Yellow
$createBody = @{
    name = "Cơm trắng"
    category = "Grains"
    calories = 130
    protein = 2.7
    carbs = 28.2
    fat = 0.3
    servingSize = 100
    imageUrl = $null
    description = "Cơm trắng Việt Nam"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "$baseUrl/Food" -Method Post -Body $createBody -ContentType "application/json"
Write-Host "Created Food:" -ForegroundColor Green
$response | ConvertTo-Json
$foodId = $response.data.id

# 2. Get All Foods
Write-Host "`n2. Getting all foods..." -ForegroundColor Yellow
$allFoods = Invoke-RestMethod -Uri "$baseUrl/Food" -Method Get
Write-Host "All Foods:" -ForegroundColor Green
$allFoods | ConvertTo-Json

# 3. Get Food by ID
Write-Host "`n3. Getting food by ID: $foodId..." -ForegroundColor Yellow
$food = Invoke-RestMethod -Uri "$baseUrl/Food/$foodId" -Method Get
Write-Host "Food Detail:" -ForegroundColor Green
$food | ConvertTo-Json

# 4. Update Food
Write-Host "`n4. Updating food..." -ForegroundColor Yellow
$updateBody = @{
    name = "Cơm trắng (Updated)"
    category = "Grains"
    calories = 195
    protein = 4.0
    carbs = 42.3
    fat = 0.5
    servingSize = 150
    imageUrl = "https://example.com/rice.jpg"
    description = "Cơm trắng Việt Nam - Updated"
} | ConvertTo-Json

$updated = Invoke-RestMethod -Uri "$baseUrl/Food/$foodId" -Method Put -Body $updateBody -ContentType "application/json"
Write-Host "Updated Food:" -ForegroundColor Green
$updated | ConvertTo-Json

# 5. Search Foods
Write-Host "`n5. Searching foods with 'cơm'..." -ForegroundColor Yellow
$searched = Invoke-RestMethod -Uri "$baseUrl/Food?search=cơm" -Method Get
Write-Host "Search Results:" -ForegroundColor Green
$searched | ConvertTo-Json

# 6. Delete Food
Write-Host "`n6. Deleting food..." -ForegroundColor Yellow
$deleted = Invoke-RestMethod -Uri "$baseUrl/Food/$foodId" -Method Delete
Write-Host "Deleted:" -ForegroundColor Green
$deleted | ConvertTo-Json

Write-Host "`n=== All tests completed! ===" -ForegroundColor Green
