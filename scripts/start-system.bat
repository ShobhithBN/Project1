@echo off
echo ================================================
echo AI Exam Proctor System Startup Script
echo ================================================

REM Check if Docker is running
echo Checking Docker status...
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Docker is not running or not installed!
    echo Please start Docker Desktop and try again.
    pause
    exit /b 1
)

echo Docker is running. Checking Docker Compose...
docker-compose --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Docker Compose is not available!
    echo Please install Docker Compose and try again.
    pause
    exit /b 1
)

echo Both Docker and Docker Compose are available.
echo.

REM Navigate to project directory
cd /d "%~dp0.."

echo Starting AI Proctoring System...
echo.

echo Step 1: Starting database services...
docker-compose up -d postgres redis
if %errorlevel% neq 0 (
    echo ERROR: Failed to start database services!
    pause
    exit /b 1
)

echo Waiting 30 seconds for databases to initialize...
timeout /t 30 /nobreak

echo Step 2: Starting backend services...
docker-compose up -d auth-service proctoring-service analytics-service notifications-service
if %errorlevel% neq 0 (
    echo ERROR: Failed to start backend services!
    echo Checking logs...
    docker-compose logs --tail=20
    pause
    exit /b 1
)

echo Step 3: Starting AI service...
docker-compose up -d ai-service
if %errorlevel% neq 0 (
    echo ERROR: Failed to start AI service!
    echo Checking logs...
    docker-compose logs ai-service --tail=20
    pause
    exit /b 1
)

echo Step 4: Starting API Gateway (optional)...
docker-compose up -d nginx

echo.
echo ================================================
echo System startup complete!
echo ================================================

echo Checking service status...
docker-compose ps

echo.
echo Service URLs:
echo - Authentication: http://localhost:7001
echo - Proctoring: http://localhost:7002
echo - Analytics: http://localhost:7003
echo - Notifications: http://localhost:7004
echo - AI Service: http://localhost:8000
echo - Gateway: http://localhost
echo.

echo To view logs: docker-compose logs -f
echo To stop system: docker-compose down
echo.
pause