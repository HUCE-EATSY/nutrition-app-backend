# Simple Test - Get All Foods
$baseUrl = "http://localhost:5184/api/v1"

Write-Host "Testing GET All Foods..." -ForegroundColor Yellow

try {
    $response = Invoke-WebRequest -Uri "$baseUrl/Food" -Method Get -UseBasicParsing
    Write-Host "Status Code: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "Response:" -ForegroundColor Green
    $response.Content | ConvertFrom-Json | ConvertTo-Json -Depth 10
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body:" -ForegroundColor Red
        Write-Host $responseBody
    }
}

Write-Host "`n---`n" -ForegroundColor Gray

Write-Host "Testing POST Create Food..." -ForegroundColor Yellow

$body = @{
    name = "Com trang"
    category = "Grains"
    calories = 130
    protein = 2.7
    carbs = 28.2
    fat = 0.3
    servingSize = 100
    imageUrl = $null
    description = "Vietnamese white rice"
} | ConvertTo-Json

Write-Host "Request Body:" -ForegroundColor Cyan
Write-Host $body

try {
    $response = Invoke-WebRequest -Uri "$baseUrl/Food" -Method Post -Body $body -ContentType "application/json" -UseBasicParsing
    Write-Host "Status Code: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "Response:" -ForegroundColor Green
    $response.Content | ConvertFrom-Json | ConvertTo-Json -Depth 10
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body:" -ForegroundColor Red
        Write-Host $responseBody
    }
}
