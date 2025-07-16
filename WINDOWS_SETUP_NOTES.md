# 🪟 Windows-Specific Setup Notes for AI Proctoring System

This document provides Windows-specific instructions to complement the main VSCode setup guide.

## 🔧 Windows Prerequisites

### 1. Enable WSL 2 (Recommended for Docker)

**Open PowerShell as Administrator:**
```powershell
# Install WSL 2
wsl --install

# Set WSL 2 as default
wsl --set-default-version 2

# Restart your computer
```

### 2. Install Docker Desktop for Windows

1. **Download:** https://www.docker.com/products/docker-desktop
2. **Install** with these settings:
   - ✅ Use WSL 2 instead of Hyper-V
   - ✅ Add path to Windows environment
3. **Restart** your computer
4. **Start Docker Desktop** and wait for it to complete startup

### 3. Install .NET 8.0 SDK

**Option A: Winget (Windows Package Manager)**
```powershell
winget install Microsoft.DotNet.SDK.8
```

**Option B: Direct Download**
- Download from: https://dotnet.microsoft.com/download/dotnet/8.0
- Choose "Windows x64 Installer"

### 4. Install Visual Studio Code

```powershell
winget install Microsoft.VisualStudioCode
```

## 🚀 Windows-Specific Commands

### PowerShell Commands (use these instead of bash)

```powershell
# Navigate to project
cd "E:\AI exam proctor system\Project1"

# Check installations
dotnet --version
docker --version
docker-compose --version

# Make sure Docker Desktop is running (check system tray)
# Start services
docker-compose up -d

# Check services
docker-compose ps
docker-compose logs -f

# Build .NET solution
dotnet build AIProctoring.sln

# Run MAUI client (Windows)
cd src\Client
dotnet run --framework net8.0-windows10.0.19041.0
```

### File Path Considerations

- Windows uses backslashes (`\`) in paths
- Use quotes around paths with spaces
- Example: `"E:\AI exam proctor system\Project1"`

## 🛠️ Windows Development Setup

### PowerShell Execution Policy

If you get execution policy errors:
```powershell
# Run as Administrator
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Visual Studio Code Terminal Setup

**Set PowerShell as Default Terminal:**
1. Open VSCode
2. `Ctrl+Shift+P` → "Terminal: Select Default Profile"
3. Choose "PowerShell"

**Or configure in settings.json:**
```json
{
    "terminal.integrated.defaultProfile.windows": "PowerShell",
    "terminal.integrated.profiles.windows": {
        "PowerShell": {
            "source": "PowerShell",
            "icon": "terminal-powershell"
        }
    }
}
```

## 🔍 Troubleshooting Windows Issues

### Docker Desktop Issues

**Problem: "Docker Desktop is not running"**
```powershell
# Check if Docker service is running
Get-Service -Name "*docker*"

# Start Docker Desktop manually
& "C:\Program Files\Docker\Docker\Docker Desktop.exe"
```

**Problem: WSL 2 Issues**
```powershell
# Update WSL
wsl --update

# Check WSL version
wsl --list --verbose

# Restart WSL
wsl --shutdown
```

**Problem: Hyper-V Conflicts**
1. Open "Turn Windows features on or off"
2. Disable "Hyper-V" if using WSL 2
3. Enable "Virtual Machine Platform" and "Windows Subsystem for Linux"
4. Restart computer

### Port Conflicts on Windows

```powershell
# Check what's using a port
netstat -ano | findstr :7001

# Kill process using port (replace PID with actual process ID)
taskkill /PID <PID> /F
```

### .NET MAUI on Windows

**Install Windows App SDK:**
```powershell
# Install via winget
winget install Microsoft.WindowsAppSDK
```

**For Windows development, install workloads:**
```powershell
dotnet workload install maui-windows
dotnet workload install maui-android  # Optional for Android development
```

### Firewall Settings

If services can't communicate:
1. Open "Windows Defender Firewall"
2. Click "Allow an app or feature through Windows Defender Firewall"
3. Add Docker Desktop and .NET applications

## 📁 Windows Project Structure

```
E:\AI exam proctor system\Project1\
├── .vscode\                 # VSCode configuration
├── src\
│   ├── Client\             # MAUI Cross-platform client
│   ├── Services\           # Backend microservices
│   └── Shared\            # Shared libraries
├── scripts\               # Setup scripts
├── docker-compose.yml     # Service orchestration
└── AIProctoring.sln      # .NET solution file
```

## 🎯 Windows-Specific VSCode Tasks

Add these to your `.vscode/tasks.json`:

```json
{
    "label": "run-client-windows",
    "command": "dotnet",
    "type": "process",
    "args": ["run", "--project", "${workspaceFolder}\\src\\Client", "--framework", "net8.0-windows10.0.19041.0"],
    "group": "build",
    "presentation": {
        "echo": true,
        "reveal": "always",
        "focus": true,
        "panel": "new"
    },
    "options": {
        "cwd": "${workspaceFolder}"
    }
},
{
    "label": "build-windows",
    "command": "dotnet",
    "type": "process",
    "args": ["build", "${workspaceFolder}\\AIProctoring.sln", "--configuration", "Debug"],
    "group": "build",
    "presentation": {
        "echo": true,
        "reveal": "silent",
        "focus": false,
        "panel": "shared"
    },
    "problemMatcher": "$msCompile"
}
```

## 🔐 Windows Security Considerations

### Antivirus Exclusions

Add these folders to your antivirus exclusions:
- `E:\AI exam proctor system\Project1\`
- `%USERPROFILE%\.docker\`
- `%USERPROFILE%\.dotnet\`
- `%USERPROFILE%\.nuget\`

### User Account Control (UAC)

Some operations may require administrator privileges:
- Installing software
- Modifying system settings
- First-time Docker setup

## 🎮 Quick Start for Windows

**One-command setup (run as Administrator):**
```powershell
# Install prerequisites
winget install Microsoft.DotNet.SDK.8
winget install Docker.DockerDesktop
winget install Microsoft.VisualStudioCode

# Restart computer, then:
cd "E:\AI exam proctor system\Project1"
docker-compose up -d
dotnet build AIProctoring.sln
code .
```

## 📞 Getting Help on Windows

**Check system status:**
```powershell
# System information
systeminfo | findstr /B /C:"OS Name" /C:"OS Version"

# Check .NET installations
dotnet --list-sdks
dotnet --list-runtimes

# Check Docker
docker version
docker-compose version
```

**Common Windows error solutions:**
- **Path too long**: Enable long path support in Windows
- **Permission denied**: Run as Administrator
- **Port in use**: Check with `netstat -ano | findstr :<port>`
- **Service not found**: Restart Windows services

---

💡 **Tip**: Consider using Windows Terminal for a better command-line experience with PowerShell, Command Prompt, and WSL all in one place!