#!/usr/bin/env pwsh

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "AI Exam Proctor System Startup Script" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host

# Function to check if Docker is running
function Test-DockerRunning {
    try {
        $null = docker version 2>$null
        return $true
    }
    catch {
        return $false
    }
}

# Function to wait for service to be ready
function Wait-ForService {
    param($ServiceName, $Port, $MaxAttempts = 30)
    
    Write-Host "Waiting for $ServiceName to be ready on port $Port..." -ForegroundColor Yellow
    
    for ($i = 1; $i -le $MaxAttempts; $i++) {
        try {
            $connection = Test-NetConnection -ComputerName localhost -Port $Port -InformationLevel Quiet -ErrorAction SilentlyContinue
            if ($connection) {
                Write-Host "$ServiceName is ready!" -ForegroundColor Green
                return $true
            }
        }
        catch {
            # Continue waiting
        }
        
        Write-Host "Attempt $i/$MaxAttempts - $ServiceName not ready yet..." -ForegroundColor Gray
        Start-Sleep -Seconds 2
    }
    
    Write-Host "Warning: $ServiceName may not be fully ready" -ForegroundColor Yellow
    return $false
}

# Check Docker status
Write-Host "Checking Docker status..." -ForegroundColor White
if (-not (Test-DockerRunning)) {
    Write-Host "ERROR: Docker is not running!" -ForegroundColor Red
    Write-Host "Please ensure Docker Desktop is running and try again." -ForegroundColor Red
    Write-Host
    Write-Host "To fix this:" -ForegroundColor Yellow
    Write-Host "1. Start Docker Desktop" -ForegroundColor Yellow
    Write-Host "2. Wait for the green whale icon in system tray" -ForegroundColor Yellow
    Write-Host "3. Run this script again" -ForegroundColor Yellow
    exit 1
}

Write-Host "✓ Docker is running" -ForegroundColor Green

# Check Docker Compose
try {
    $null = docker-compose --version 2>$null
    Write-Host "✓ Docker Compose is available" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: Docker Compose is not available!" -ForegroundColor Red
    exit 1
}

# Navigate to project directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectPath = Split-Path -Parent $scriptPath
Set-Location $projectPath

Write-Host
Write-Host "Starting AI Proctoring System from: $projectPath" -ForegroundColor White
Write-Host

# Step 1: Database services
Write-Host "Step 1: Starting database services..." -ForegroundColor Cyan
try {
    docker-compose up -d postgres redis
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to start database services"
    }
    Write-Host "✓ Database services started" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: Failed to start database services!" -ForegroundColor Red
    Write-Host "Running diagnostics..." -ForegroundColor Yellow
    docker-compose logs postgres --tail=10
    docker-compose logs redis --tail=10
    exit 1
}

# Wait for databases
Write-Host "Waiting for databases to initialize..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Step 2: Backend services
Write-Host "Step 2: Starting backend services..." -ForegroundColor Cyan
try {
    docker-compose up -d auth-service proctoring-service analytics-service notifications-service
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to start backend services"
    }
    Write-Host "✓ Backend services started" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: Failed to start backend services!" -ForegroundColor Red
    Write-Host "Checking logs..." -ForegroundColor Yellow
    docker-compose logs auth-service --tail=10
    docker-compose logs proctoring-service --tail=10
    exit 1
}

# Step 3: AI service
Write-Host "Step 3: Starting AI service..." -ForegroundColor Cyan
try {
    docker-compose up -d ai-service
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to start AI service"
    }
    Write-Host "✓ AI service started" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: Failed to start AI service!" -ForegroundColor Red
    Write-Host "Checking logs..." -ForegroundColor Yellow
    docker-compose logs ai-service --tail=10
    exit 1
}

# Step 4: API Gateway (optional)
Write-Host "Step 4: Starting API Gateway..." -ForegroundColor Cyan
try {
    docker-compose up -d nginx
    Write-Host "✓ API Gateway started" -ForegroundColor Green
}
catch {
    Write-Host "Warning: API Gateway failed to start (this is optional)" -ForegroundColor Yellow
}

Write-Host
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "System startup complete!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan

# Check service status
Write-Host
Write-Host "Service Status:" -ForegroundColor White
docker-compose ps

Write-Host
Write-Host "Service URLs:" -ForegroundColor White
Write-Host "- Authentication: http://localhost:7001" -ForegroundColor Gray
Write-Host "- Proctoring: http://localhost:7002" -ForegroundColor Gray
Write-Host "- Analytics: http://localhost:7003" -ForegroundColor Gray
Write-Host "- Notifications: http://localhost:7004" -ForegroundColor Gray
Write-Host "- AI Service: http://localhost:8000" -ForegroundColor Gray
Write-Host "- Gateway: http://localhost" -ForegroundColor Gray

Write-Host
Write-Host "Health Check Commands:" -ForegroundColor White
Write-Host "docker-compose logs -f" -ForegroundColor Gray
Write-Host "docker-compose ps" -ForegroundColor Gray
Write-Host "docker-compose down" -ForegroundColor Gray

# Optional health checks
Write-Host
Write-Host "Running basic health checks..." -ForegroundColor Yellow

$services = @{
    "PostgreSQL" = 5432
    "Redis" = 6379
    "Auth Service" = 7001
    "Proctoring Service" = 7002
    "AI Service" = 8000
}

foreach ($service in $services.GetEnumerator()) {
    try {
        $connection = Test-NetConnection -ComputerName localhost -Port $service.Value -InformationLevel Quiet -ErrorAction SilentlyContinue
        if ($connection) {
            Write-Host "✓ $($service.Key) is responding on port $($service.Value)" -ForegroundColor Green
        } else {
            Write-Host "⚠ $($service.Key) may not be ready on port $($service.Value)" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "⚠ Could not check $($service.Key)" -ForegroundColor Yellow
    }
}

Write-Host
Write-Host "System is ready! You can now start the MAUI client application." -ForegroundColor Green