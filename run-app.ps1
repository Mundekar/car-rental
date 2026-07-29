param([switch]$NoWait = $false)

Write-Host ""
Write-Host "=========================================="
Write-Host "Car Rental App Launcher"
Write-Host "=========================================="
Write-Host ""

# Check project structure
if (-not (Test-Path "src/CarRental.Api/Program.cs")) {
    Write-Host "ERROR: Must run from project root directory" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path "car-rental-ui/package.json")) {
    Write-Host "ERROR: Frontend directory not found" -ForegroundColor Red
    exit 1
}

Write-Host "OK: Project structure verified" -ForegroundColor Green
Write-Host ""

# Check prerequisites
Write-Host "Checking prerequisites..."

$dotnetVersion = dotnet --version 2>$null
if (-not $dotnetVersion) {
    Write-Host "ERROR: .NET SDK not found" -ForegroundColor Red
    exit 1
}
Write-Host "OK: .NET SDK $dotnetVersion" -ForegroundColor Green

$nodeVersion = node --version 2>$null
if (-not $nodeVersion) {
    Write-Host "ERROR: Node.js not found" -ForegroundColor Red
    exit 1
}
Write-Host "OK: Node.js $nodeVersion" -ForegroundColor Green
Write-Host ""

$projectRoot = Get-Location

# Start Backend
Write-Host "Starting Backend..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$projectRoot'; Write-Host 'Backend starting...'; dotnet run --project src/CarRental.Api --configuration Release"

Start-Sleep -Seconds 4

# Start Frontend  
Write-Host "Starting Frontend..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$projectRoot/car-rental-ui'; Write-Host 'Frontend starting...'; npm run dev"

Write-Host ""
Write-Host "=========================================="
Write-Host "Application Started!"
Write-Host "=========================================="
Write-Host ""
Write-Host "Backend:  http://localhost:5000" -ForegroundColor Green
Write-Host "Swagger:  http://localhost:5000/swagger" -ForegroundColor Green
Write-Host "Frontend: http://localhost:3000" -ForegroundColor Green
Write-Host ""
Write-Host "Waiting for services to be ready..."

$maxAttempts = 30
$attempt = 0
$backendReady = $false
$frontendReady = $false

while (($attempt -lt $maxAttempts) -and (-not ($backendReady -and $frontendReady))) {
    $attempt++
    
    if (-not $backendReady) {
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:5000/swagger" -TimeoutSec 2 -UseBasicParsing -ErrorAction SilentlyContinue
            if ($response.StatusCode -eq 200) {
                $backendReady = $true
                Write-Host "OK: Backend is ready" -ForegroundColor Green
            }
        }
        catch {
            # Waiting
        }
    }
    
    if (-not $frontendReady) {
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:3000" -TimeoutSec 2 -UseBasicParsing -ErrorAction SilentlyContinue
            if ($response.StatusCode -eq 200) {
                $frontendReady = $true
                Write-Host "OK: Frontend is ready" -ForegroundColor Green
            }
        }
        catch {
            # Waiting
        }
    }
    
    if (-not ($backendReady -and $frontendReady)) {
        Start-Sleep -Seconds 1
    }
}

Write-Host ""
Write-Host "=========================================="
Write-Host "Ready to Use!"
Write-Host "=========================================="
Write-Host ""
Write-Host "Open: http://localhost:3000" -ForegroundColor Green
Write-Host ""

if (-not $NoWait) {
    Write-Host "Press any key to exit launcher..."
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
}
