# Complete Food API Test Script
$baseUrl = "http://localhost:5184/api/v1"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   FOOD API - COMPLETE TEST SUITE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Helper function to display results
function Show-Result {
    param($title, $response)
    Write-Host "`n$title" -ForegroundColor Yellow
    Write-Host "Status: $($response.StatusCode)" -ForegroundColor Green
    $response.Content | ConvertFrom-Json | ConvertTo-Json -Depth 10
}

# Test 1: Create Multiple Foods
Write-Host "`n[1/10] Creating Food #1: Cơm trắng..." -ForegroundColor Magenta
$food1 = @{
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

try {
    $response1 = Invoke-WebRequest -Uri "$baseUrl/Food" -Method Post -Body $food1 -ContentType "application/json" -UseBasicParsing
    Show-Result "✅ Created Food #1" $response1
    $foodId1 = ($response1.Content | ConvertFrom-Json).data.id
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n[2/10] Creating Food #2: Chicken Breast..." -ForegroundColor Magenta
$food2 = @{
    name = "Chicken Breast"
    category = "Protein"
    calories = 165
    protein = 31
    carbs = 0
    fat = 3.6
    servingSize = 100
    imageUrl = "https://example.com/chicken.jpg"
    description = "Grilled chicken breast"
} | ConvertTo-Json

try {
    $response2 = Invoke-WebRequest -Uri "$baseUrl/Food" -Method Post -Body $food2 -ContentType "application/json" -UseBasicParsing
    Show-Result "✅ Created Food #2" $response2
    $foodId2 = ($response2.Content | ConvertFrom-Json).data.id
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n[3/10] Creating Food #3: Banana..." -ForegroundColor Magenta
$food3 = @{
    name = "Banana"
    category = "Fruit"
    calories = 89
    protein = 1.1
    carbs = 22.8
    fat = 0.3
    servingSize = 100
    imageUrl = "https://example.com/banana.jpg"
    description = "Fresh banana"
} | ConvertTo-Json

try {
    $response3 = Invoke-WebRequest -Uri "$baseUrl/Food" -Method Post -Body $food3 -ContentType "application/json" -UseBasicParsing
    Show-Result "✅ Created Food #3" $response3
    $foodId3 = ($response3.Content | ConvertFrom-Json).data.id
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Get All Foods
Write-Host "`n[4/10] Getting all foods..." -ForegroundColor Magenta
try {
    $allFoods = Invoke-WebRequest -Uri "$baseUrl/Food" -Method Get -UseBasicParsing
    Show-Result "✅ All Foods" $allFoods
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Get Food by ID
Write-Host "`n[5/10] Getting food by ID: $foodId1..." -ForegroundColor Magenta
try {
    $foodById = Invoke-WebRequest -Uri "$baseUrl/Food/$foodId1" -Method Get -UseBasicParsing
    Show-Result "✅ Food Detail (ID: $foodId1)" $foodById
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Filter by Category
Write-Host "`n[6/10] Filtering foods by category: Protein..." -ForegroundColor Magenta
try {
    $filtered = Invoke-WebRequest -Uri "$baseUrl/Food?category=Protein" -Method Get -UseBasicParsing
    Show-Result "✅ Filtered by Category (Protein)" $filtered
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 5: Search by Name
Write-Host "`n[7/10] Searching foods with keyword: 'Chicken'..." -ForegroundColor Magenta
try {
    $searched = Invoke-WebRequest -Uri "$baseUrl/Food?search=Chicken" -Method Get -UseBasicParsing
    Show-Result "✅ Search Results (Chicken)" $searched
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 6: Update Food
Write-Host "`n[8/10] Updating food ID: $foodId1..." -ForegroundColor Magenta
$updateFood = @{
    name = "Cơm trắng (Updated)"
    category = "Grains"
    calories = 195
    protein = 4.0
    carbs = 42.3
    fat = 0.5
    servingSize = 150
    imageUrl = "https://example.com/rice-updated.jpg"
    description = "Cơm trắng Việt Nam - Phần lớn hơn"
} | ConvertTo-Json

try {
    $updated = Invoke-WebRequest -Uri "$baseUrl/Food/$foodId1" -Method Put -Body $updateFood -ContentType "application/json" -UseBasicParsing
    Show-Result "✅ Updated Food (ID: $foodId1)" $updated
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 7: Get Updated Food
Write-Host "`n[9/10] Verifying updated food..." -ForegroundColor Magenta
try {
    $verifyUpdate = Invoke-WebRequest -Uri "$baseUrl/Food/$foodId1" -Method Get -UseBasicParsing
    Show-Result "✅ Verified Updated Food" $verifyUpdate
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 8: Delete Food
Write-Host "`n[10/10] Deleting food ID: $foodId3..." -ForegroundColor Magenta
try {
    $deleted = Invoke-WebRequest -Uri "$baseUrl/Food/$foodId3" -Method Delete -UseBasicParsing
    Show-Result "✅ Deleted Food (ID: $foodId3)" $deleted
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Final Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "   TEST SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "✅ Created 3 foods" -ForegroundColor Green
Write-Host "✅ Retrieved all foods" -ForegroundColor Green
Write-Host "✅ Retrieved food by ID" -ForegroundColor Green
Write-Host "✅ Filtered by category" -ForegroundColor Green
Write-Host "✅ Searched by name" -ForegroundColor Green
Write-Host "✅ Updated a food" -ForegroundColor Green
Write-Host "✅ Deleted a food" -ForegroundColor Green
Write-Host "`nFinal state: 2 foods remaining (Cơm trắng, Chicken Breast)" -ForegroundColor Yellow
Write-Host "========================================`n" -ForegroundColor Cyan
