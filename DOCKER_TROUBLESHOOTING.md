# Docker Troubleshooting Guide - AI Exam Proctor System

## Issue Overview
You're experiencing Docker connectivity issues with the error:
```
error during connect: Get "http://%2F%2F.%2Fpipe%2FdockerDesktopLinuxEngine/v1.51/containers/json": open //./pipe/dockerDesktopLinuxEngine: The system cannot find the file specified.
```

This indicates that Docker Desktop is not running or there's a connectivity issue with the Docker daemon.

## Solutions

### 1. Immediate Solutions (Windows)

#### Solution A: Restart Docker Desktop
1. **Close Docker Desktop completely:**
   - Right-click on Docker Desktop icon in system tray
   - Select "Quit Docker Desktop"
   - Wait 10 seconds

2. **Restart Docker Desktop:**
   - Launch Docker Desktop as Administrator
   - Wait for it to fully start (green icon in system tray)
   - Try your commands again

#### Solution B: Reset Docker Desktop
1. **Open Docker Desktop Settings:**
   - Right-click Docker Desktop icon → Settings
   - Go to "Troubleshoot" tab
   - Click "Reset to factory defaults"
   - Restart Docker Desktop

#### Solution C: Check Docker Service
1. **Open Services (Windows):**
   ```cmd
   services.msc
   ```
2. **Find and restart these services:**
   - Docker Desktop Service
   - com.docker.service
3. **Set them to "Automatic" startup**

### 2. Alternative Solutions

#### Use Docker without Docker Desktop
If Docker Desktop continues to have issues, you can use Docker directly:

1. **Install Docker Engine on WSL2:**
   ```bash
   # In WSL2 terminal
   curl -fsSL https://get.docker.com -o get-docker.sh
   sudo sh get-docker.sh
   sudo usermod -aG docker $USER
   ```

2. **Install Docker Compose:**
   ```bash
   sudo apt update
   sudo apt install docker-compose-plugin
   ```

3. **Start Docker service:**
   ```bash
   sudo service docker start
   ```

### 3. Project-Specific Fixes

#### Fixed docker-compose.yml
The `version` attribute has been removed from your docker-compose.yml file to eliminate the warning.

#### Running the AI Proctor System

1. **Basic startup sequence:**
   ```cmd
   # Ensure Docker is running
   docker --version
   docker-compose --version
   
   # Start database services first
   docker-compose up -d postgres redis
   
   # Wait 30 seconds for databases to initialize
   timeout /t 30
   
   # Start backend services
   docker-compose up -d auth-service proctoring-service analytics-service
   
   # Start AI service
   docker-compose up -d ai-service
   
   # Start gateway (optional)
   docker-compose up -d nginx
   ```

2. **Check service status:**
   ```cmd
   docker-compose ps
   docker-compose logs -f
   ```

3. **If services fail to start:**
   ```cmd
   # Check individual service logs
   docker-compose logs auth-service
   docker-compose logs postgres
   docker-compose logs ai-service
   ```

### 4. Development Mode

For development, you can start services individually:

```cmd
# Database only
docker-compose up -d postgres redis

# Then run backend services locally in your IDE
# This is useful for debugging
```

### 5. Port Conflicts

If you get port conflicts:

1. **Check what's using the ports:**
   ```cmd
   netstat -ano | findstr :5432
   netstat -ano | findstr :6379
   netstat -ano | findstr :7001
   ```

2. **Kill processes or change ports in docker-compose.yml**

### 6. System Requirements Check

Ensure your system meets requirements:
- **RAM:** Minimum 8GB (16GB recommended)
- **Storage:** 20GB free space
- **Windows:** Windows 10/11 with WSL2 enabled
- **Docker:** Version 20.0+

### 7. Complete Reset Procedure

If nothing else works:

1. **Stop all containers:**
   ```cmd
   docker-compose down -v
   docker system prune -a -f
   ```

2. **Reset Docker Desktop completely**

3. **Restart your computer**

4. **Start fresh:**
   ```cmd
   docker-compose up -d postgres redis
   # Wait 1 minute
   docker-compose up -d
   ```

## Service Architecture

Your AI Proctor system includes:
- **postgres**: PostgreSQL database (port 5432)
- **redis**: Redis cache (port 6379)
- **auth-service**: Authentication (port 7001)
- **proctoring-service**: Main proctoring logic (port 7002)
- **analytics-service**: Analytics engine (port 7003)
- **notifications-service**: Real-time notifications (port 7004)
- **ai-service**: Python AI/ML service (port 8000)
- **nginx**: API gateway (ports 80/443)

## Quick Health Check

Once Docker is working, verify your system:

```cmd
# Check if services are responding
curl http://localhost:7001/health
curl http://localhost:7002/health
curl http://localhost:8000/health
```

## Next Steps

1. Fix Docker connectivity using solutions above
2. Start with database services only
3. Gradually add other services
4. Check logs for any application-specific issues
5. Test the MAUI client connection

If you continue to have issues, try running in WSL2 or consider using a Linux virtual machine for development.