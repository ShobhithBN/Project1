# 🔧 Complete VSCode Setup Guide for AI-Proctored Online Exam System

This guide provides step-by-step instructions to set up and run the AI-Proctored Online Exam System using Visual Studio Code on Linux, macOS, or Windows.

## 📋 Prerequisites & System Requirements

### Required Software
- **Visual Studio Code** (latest version)
- **Docker** (version 20.0 or higher)
- **Docker Compose** (version 2.0 or higher)
- **.NET 8.0 SDK**
- **Git**
- **Python 3.8+** (for AI services)

### Platform Requirements
- **Linux**: Ubuntu 18.04+, CentOS 7+, or equivalent
- **macOS**: macOS 10.15 or later
- **Windows**: Windows 10/11 with WSL2 or Windows Server 2019/2022

## 🛠️ Step 1: Install Prerequisites

### Install .NET 8.0 SDK

**Linux (Ubuntu/Debian):**
```bash
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

**macOS:**
```bash
# Using Homebrew
brew install --cask dotnet

# Or download from Microsoft
# https://dotnet.microsoft.com/download/dotnet/8.0
```

**Windows:**
Download and install from: https://dotnet.microsoft.com/download/dotnet/8.0

### Install Docker

**Linux:**
```bash
# Ubuntu/Debian
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER
```

**macOS:**
Download Docker Desktop from: https://www.docker.com/products/docker-desktop

**Windows:**
Download Docker Desktop from: https://www.docker.com/products/docker-desktop

### Verify Installations
```bash
dotnet --version  # Should show 8.0.x
docker --version  # Should show 20.0+
docker-compose --version  # Should show 2.0+
python3 --version  # Should show 3.8+
```

## 🚀 Step 2: Clone and Setup Project

### Clone Repository
```bash
git clone <repository-url>
cd ai-proctoring-system
```

### Make Setup Script Executable (Linux/macOS)
```bash
chmod +x scripts/setup.sh
```

## 🔌 Step 3: VSCode Extensions Setup

### Essential Extensions

Open VSCode and install these extensions:

1. **C# Dev Kit** (`ms-dotnettools.csdevkit`) - For .NET development
2. **C#** (`ms-dotnettools.csharp`) - C# language support
3. **Docker** (`ms-azuretools.vscode-docker`) - Docker support
4. **Python** (`ms-python.python`) - Python language support
5. **REST Client** (`humao.rest-client`) - For API testing
6. **GitLens** (`eamodio.gitlens`) - Enhanced Git capabilities
7. **Thunder Client** (`rangav.vscode-thunder-client`) - API testing (alternative to Postman)

### MAUI-Specific Extensions
8. **.NET MAUI** (`ms-dotnettools.dotnet-maui`) - MAUI development support
9. **XAML** (`ms-dotnettools.xaml`) - XAML language support

### Optional but Recommended
10. **Prettier** (`esbenp.prettier-vscode`) - Code formatting
11. **Error Lens** (`usernamehw.errorlens`) - Inline error display
12. **Auto Rename Tag** (`formulahendry.auto-rename-tag`) - For XAML editing

### Install Extensions via Command Line
```bash
# Install all essential extensions at once
code --install-extension ms-dotnettools.csdevkit
code --install-extension ms-dotnettools.csharp
code --install-extension ms-azuretools.vscode-docker
code --install-extension ms-python.python
code --install-extension humao.rest-client
code --install-extension eamodio.gitlens
code --install-extension ms-dotnettools.dotnet-maui
code --install-extension ms-dotnettools.xaml
```

## 🏗️ Step 4: Project Setup in VSCode

### Open Project in VSCode
```bash
# From the project root directory
code .
```

### Configure VSCode Workspace

Create `.vscode/settings.json` (VSCode will create this folder):
```json
{
    "dotnet.defaultSolution": "AIProctoring.sln",
    "omnisharp.enableRoslynAnalyzers": true,
    "omnisharp.enableEditorConfigSupport": true,
    "python.defaultInterpreterPath": "/usr/bin/python3",
    "files.exclude": {
        "**/bin": true,
        "**/obj": true,
        "**/.dockerignore": false,
        "**/Dockerfile": false
    },
    "search.exclude": {
        "**/bin": true,
        "**/obj": true,
        "**/node_modules": true
    }
}
```

### Configure Launch Settings

Create `.vscode/launch.json`:
```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "MAUI Client",
            "type": "coreclr",
            "request": "launch",
            "program": "${workspaceFolder}/src/Client/bin/Debug/net8.0-maccatalyst/AIProctoring.Client.dll",
            "args": [],
            "cwd": "${workspaceFolder}/src/Client",
            "stopAtEntry": false,
            "console": "internalConsole"
        },
        {
            "name": "Auth Service",
            "type": "coreclr",
            "request": "launch",
            "program": "${workspaceFolder}/src/Services/Authentication/bin/Debug/net8.0/AIProctoring.Authentication.dll",
            "args": [],
            "cwd": "${workspaceFolder}/src/Services/Authentication",
            "stopAtEntry": false,
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development",
                "ASPNETCORE_URLS": "https://localhost:7001"
            }
        }
    ]
}
```

### Configure Tasks

Create `.vscode/tasks.json`:
```json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "build-solution",
            "command": "dotnet",
            "type": "process",
            "args": ["build", "${workspaceFolder}/AIProctoring.sln"],
            "group": "build",
            "presentation": {
                "echo": true,
                "reveal": "silent",
                "focus": false,
                "panel": "shared"
            },
            "problemMatcher": "$msCompile"
        },
        {
            "label": "run-client",
            "command": "dotnet",
            "type": "process",
            "args": ["run", "--project", "${workspaceFolder}/src/Client"],
            "group": "build",
            "presentation": {
                "echo": true,
                "reveal": "always",
                "focus": true,
                "panel": "new"
            }
        },
        {
            "label": "docker-up",
            "command": "docker-compose",
            "type": "shell",
            "args": ["up", "-d"],
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
            "label": "docker-down",
            "command": "docker-compose",
            "type": "shell",
            "args": ["down"],
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
        }
    ]
}
```

## 🐳 Step 5: Start Backend Services

### Option A: Automated Setup (Recommended)
```bash
# Run the automated setup script
./scripts/setup.sh
```

### Option B: Manual Setup

1. **Start databases first:**
```bash
docker-compose up -d postgres redis
```

2. **Wait for databases to initialize (30 seconds), then start services:**
```bash
docker-compose up -d --build auth-service proctoring-service analytics-service notifications-service ai-service
```

3. **Start API Gateway (optional):**
```bash
docker-compose up -d nginx
```

### Verify Services are Running
```bash
# Check all containers
docker-compose ps

# Check logs
docker-compose logs -f
```

### Test Service Endpoints
```bash
# Test individual services
curl http://localhost:7001/health  # Auth Service
curl http://localhost:7002/health  # Proctoring Service
curl http://localhost:7003/health  # Analytics Service
curl http://localhost:7004/health  # Notifications Service
curl http://localhost:8000/docs    # AI Service (should show FastAPI docs)
```

## 🖥️ Step 6: Build and Run the MAUI Client

### Restore Dependencies
```bash
cd src/Client
dotnet restore
```

### Build the Solution
```bash
# From project root
dotnet build AIProctoring.sln
```

### Run the MAUI Client

**For Windows:**
```bash
cd src/Client
dotnet run --framework net8.0-windows10.0.19041.0
```

**For macOS:**
```bash
cd src/Client
dotnet run --framework net8.0-maccatalyst
```

**For general/cross-platform:**
```bash
cd src/Client
dotnet run
```

## 🔧 Step 7: Development Workflow in VSCode

### Using the Integrated Terminal

1. **Open integrated terminal** (`Ctrl+` ` or `Cmd+` `)
2. **Split terminals** for different services:
   - Terminal 1: Backend services (`docker-compose logs -f`)
   - Terminal 2: Client development (`cd src/Client && dotnet watch run`)
   - Terminal 3: General commands

### Running Tasks

Use `Ctrl+Shift+P` (or `Cmd+Shift+P`) and type "Tasks: Run Task":
- **build-solution**: Builds the entire solution
- **run-client**: Runs the MAUI client
- **docker-up**: Starts all Docker services
- **docker-down**: Stops all Docker services

### Debugging

1. **Set breakpoints** in your C# code
2. **Press F5** or use the Run and Debug panel
3. **Select configuration**: Choose "MAUI Client" or "Auth Service"
4. **Debug multiple services**: Start each service debug session separately

### Hot Reload Development

For faster development:
```bash
# Terminal 1: Keep databases running
docker-compose up postgres redis

# Terminal 2: Run auth service with hot reload
cd src/Services/Authentication && dotnet watch run

# Terminal 3: Run client with hot reload
cd src/Client && dotnet watch run
```

## 🧪 Step 8: Testing Your Setup

### Create a Test User

Create a file called `test-requests.http` in your project root:
```http
### Register a new user
POST http://localhost:7001/api/auth/register
Content-Type: application/json

{
    "email": "student@example.com",
    "password": "TestPassword123",
    "firstName": "Test",
    "lastName": "Student",
    "institution": "Test University"
}

### Login
POST http://localhost:7001/api/auth/login
Content-Type: application/json

{
    "email": "student@example.com",
    "password": "TestPassword123"
}
```

### Use REST Client Extension

1. **Install REST Client extension** (if not already installed)
2. **Open test-requests.http**
3. **Click "Send Request"** above each request
4. **Verify responses** in the right panel

### Test MAUI Client

1. **Run the client** using the task or terminal
2. **Try logging in** with the test credentials
3. **Verify UI loads** correctly
4. **Check console output** for any errors

## 🔍 Step 9: Monitoring and Debugging

### View Logs in VSCode

#### Docker Logs
```bash
# View all service logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f auth-service
docker-compose logs -f proctoring-service
```

#### Application Logs
- **Check Output panel** in VSCode
- **Use Debug Console** when debugging
- **Monitor integrated terminal** for real-time logs

### Database Access

#### PostgreSQL
```bash
# Connect to database
docker exec -it aiproctoring-postgres psql -U aiproctoring -d aiproctoring

# List databases
\l

# Connect to specific database
\c aiproctoring_auth
```

#### Redis
```bash
# Connect to Redis
docker exec -it aiproctoring-redis redis-cli

# Check keys
KEYS *
```

### Performance Monitoring

#### System Resources
```bash
# Monitor Docker container resources
docker stats

# Check disk usage
docker system df
```

## 🛠️ Step 10: Customization and Development

### Workspace Recommendations

Create `.vscode/extensions.json`:
```json
{
    "recommendations": [
        "ms-dotnettools.csdevkit",
        "ms-dotnettools.csharp",
        "ms-azuretools.vscode-docker",
        "ms-python.python",
        "ms-dotnettools.dotnet-maui",
        "ms-dotnettools.xaml",
        "humao.rest-client"
    ]
}
```

### Project Structure Overview

```
ai-proctoring-system/
├── .vscode/                 # VSCode configuration
├── src/
│   ├── Client/             # MAUI Cross-platform client
│   ├── Services/           # Backend microservices
│   │   ├── Authentication/ # User management
│   │   ├── Proctoring/     # Core proctoring logic
│   │   ├── Analytics/      # Data analysis
│   │   ├── Notifications/  # Real-time notifications
│   │   └── AI/            # Python AI service
│   └── Shared/            # Shared libraries
├── scripts/               # Setup and utility scripts
├── docker-compose.yml     # Service orchestration
└── AIProctoring.sln      # .NET solution file
```

### Common Development Tasks

#### Adding New Features
1. **Create feature branch**: `git checkout -b feature/new-feature`
2. **Modify code** in appropriate service
3. **Test changes** using hot reload
4. **Run tests**: `dotnet test`
5. **Commit and push**: Standard Git workflow

#### Database Migrations
```bash
# Add migration (example for Auth service)
cd src/Services/Authentication
dotnet ef migrations add MigrationName
dotnet ef database update
```

#### Adding New Dependencies
```bash
# Add NuGet package
dotnet add package PackageName

# Add Python package (for AI service)
cd src/Services/AI
pip install package-name
```

## 🚨 Troubleshooting Common Issues

### Port Conflicts
```bash
# Check what's using ports
sudo netstat -tulpn | grep :7001

# Change ports in docker-compose.yml if needed
```

### Build Failures
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build

# Reset Docker environment
docker-compose down
docker system prune -a
docker-compose up --build
```

### MAUI Client Issues
```bash
# Clear MAUI workload
dotnet workload restore
dotnet clean
dotnet build

# On Windows, ensure Windows App SDK is installed
```

### Database Connection Issues
```bash
# Restart database services
docker-compose restart postgres redis

# Check database logs
docker-compose logs postgres
```

### VSCode IntelliSense Issues
1. **Reload window**: `Ctrl+Shift+P` → "Developer: Reload Window"
2. **Restart OmniSharp**: `Ctrl+Shift+P` → "OmniSharp: Restart OmniSharp"
3. **Clear cache**: Delete `bin/` and `obj/` folders, then `dotnet restore`

## 📚 Additional Resources

### Documentation
- **Project docs**: Check `/docs` folder
- **API documentation**: Available at `/swagger` endpoints when services are running
- **.NET MAUI docs**: https://docs.microsoft.com/dotnet/maui/
- **ASP.NET Core docs**: https://docs.microsoft.com/aspnet/core/

### VSCode Tips
- **Command Palette**: `Ctrl+Shift+P` / `Cmd+Shift+P`
- **Quick Open**: `Ctrl+P` / `Cmd+P`
- **Integrated Terminal**: `Ctrl+` ` / `Cmd+` `
- **Debug Console**: `Ctrl+Shift+Y` / `Cmd+Shift+Y`
- **Problems Panel**: `Ctrl+Shift+M` / `Cmd+Shift+M`

### Useful Extensions for Advanced Development
- **Database Client**: `cweijan.vscode-database-client2`
- **Kubernetes**: `ms-kubernetes-tools.vscode-kubernetes-tools`
- **Azure Tools**: `ms-vscode.vscode-node-azure-pack`
- **Postman**: `postman.postman-for-vscode`

## 🔐 Security Considerations

⚠️ **Important**: This setup is for development only!

### For Production:
1. **Change all default passwords**
2. **Use HTTPS certificates**
3. **Configure proper environment variables**
4. **Set up proper authentication**
5. **Enable logging and monitoring**
6. **Follow GDPR/FERPA compliance guidelines**

## 🎯 Next Steps

1. **Explore the codebase** and understand the architecture
2. **Read the API documentation** at the Swagger endpoints
3. **Customize proctoring rules** based on your requirements
4. **Set up CI/CD pipelines** for automated testing and deployment
5. **Configure production environment** following security best practices

---

🎉 **Congratulations!** You now have a fully functional AI-Proctored Online Exam System running in VSCode. Happy coding! 🚀