# 🚀 AI Proctoring System - Quick Start Guide

Welcome to the AI-Proctored Online Exam System! This guide will help you get the system up and running in minutes.

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- **Docker** (version 20.0 or higher)
- **Docker Compose** (version 2.0 or higher)
- **.NET 8.0 SDK** (for client development)
- **Git** (for cloning the repository)

### Platform Requirements
- **Windows**: Windows 10/11 with WSL2 or Windows Server 2019/2022
- **macOS**: macOS 10.15 or later
- **Linux**: Ubuntu 18.04+, CentOS 7+, or equivalent

## ⚡ One-Command Setup

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd ai-proctoring-system
   ```

2. **Run the setup script:**
   ```bash
   ./scripts/setup.sh
   ```

That's it! The script will:
- Install missing dependencies
- Build all services
- Start the database and backend services
- Configure the API gateway
- Download required AI models

## 🏃‍♂️ Manual Setup (Alternative)

If you prefer to set up manually or the script doesn't work:

### Step 1: Start Database Services
```bash
docker-compose up -d postgres redis
```

### Step 2: Build and Start Backend Services
```bash
docker-compose up -d --build auth-service proctoring-service analytics-service notifications-service
```

### Step 3: Start AI Service
```bash
docker-compose up -d ai-service
```

### Step 4: Start API Gateway
```bash
docker-compose up -d nginx
```

### Step 5: Build the MAUI Client
```bash
cd src/Client
dotnet restore
dotnet build
```

## 🌐 Accessing the System

Once everything is running, you can access:

### Backend APIs
- **Authentication API**: http://localhost:7001/swagger
- **Proctoring API**: http://localhost:7002/swagger
- **Analytics API**: http://localhost:7003/swagger
- **Notifications API**: http://localhost:7004/swagger
- **AI Service**: http://localhost:8000/docs

### API Gateway
- **Unified API**: http://localhost/api/

### Databases
- **PostgreSQL**: localhost:5432 (user: `aiproctoring`, password: `aiproctoring123`)
- **Redis**: localhost:6379

## 🧪 Testing the System

### 1. Create a Test User
```bash
curl -X POST "http://localhost:7001/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "student@example.com",
    "password": "TestPassword123",
    "firstName": "Test",
    "lastName": "Student",
    "institution": "Test University"
  }'
```

### 2. Login
```bash
curl -X POST "http://localhost:7001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "student@example.com",
    "password": "TestPassword123"
  }'
```

### 3. Run the MAUI Client
```bash
cd src/Client
dotnet run
```

## 🔧 Development Setup

### Running Individual Services

**Authentication Service:**
```bash
cd src/Services/Authentication
dotnet run --urls="https://localhost:7001"
```

**Proctoring Service:**
```bash
cd src/Services/Proctoring
dotnet run --urls="https://localhost:7002"
```

### Hot Reload for Development
```bash
# Terminal 1: Start databases
docker-compose up postgres redis

# Terminal 2: Start auth service
cd src/Services/Authentication && dotnet watch run

# Terminal 3: Start proctoring service
cd src/Services/Proctoring && dotnet watch run

# Terminal 4: Start client
cd src/Client && dotnet run
```

## 📱 Running the MAUI Client

### Windows
```bash
cd src/Client
dotnet run --framework net8.0-windows10.0.19041.0
```

### macOS
```bash
cd src/Client
dotnet run --framework net8.0-maccatalyst
```

### Android (requires Android SDK)
```bash
cd src/Client
dotnet build --framework net8.0-android
dotnet run --framework net8.0-android
```

### iOS (requires Xcode on macOS)
```bash
cd src/Client
dotnet build --framework net8.0-ios
dotnet run --framework net8.0-ios
```

## 🎯 Creating Your First Exam

1. **Access the system** through the MAUI client or API
2. **Login** with instructor credentials
3. **Create an exam** with the following:
   - Title and description
   - Questions (multiple choice, essay, etc.)
   - Proctoring settings (face detection, audio monitoring, etc.)
   - Duration and schedule
4. **Assign students** to the exam
5. **Monitor** exam sessions in real-time

## 🔍 Monitoring and Debugging

### View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f auth-service

# Follow logs in real-time
docker-compose logs -f --tail=100
```

### Check Service Health
```bash
# Test all service endpoints
curl http://localhost:7001/health  # Auth
curl http://localhost:7002/health  # Proctoring
curl http://localhost:7003/health  # Analytics
curl http://localhost:7004/health  # Notifications
```

### Database Access
```bash
# Connect to PostgreSQL
docker exec -it aiproctoring-postgres psql -U aiproctoring -d aiproctoring_auth

# Connect to Redis
docker exec -it aiproctoring-redis redis-cli
```

## 🛠️ Common Issues & Solutions

### Port Conflicts
If you get port conflicts:
```bash
# Check what's using the ports
netstat -tulpn | grep :7001

# Stop conflicting services or change ports in docker-compose.yml
```

### Database Connection Issues
```bash
# Restart database services
docker-compose restart postgres redis

# Check database logs
docker-compose logs postgres
```

### Build Failures
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build

# For Docker issues
docker-compose down
docker system prune -a
docker-compose up --build
```

### MAUI Client Issues
```bash
# Clear MAUI cache
dotnet workload restore
dotnet clean
dotnet build

# On Windows, ensure Windows App SDK is installed
```

## 📚 Next Steps

1. **Read the Architecture Guide**: Understanding the system design
2. **Explore the API Documentation**: Available at `/swagger` endpoints
3. **Customize Proctoring Rules**: Modify detection thresholds and rules
4. **Set Up Production**: Follow the deployment guide for production setup
5. **Contribute**: Check out the contributing guidelines

## 🆘 Getting Help

- **Documentation**: Check the `/docs` folder for detailed guides
- **Issues**: Report bugs and feature requests on GitHub
- **Discussions**: Join community discussions for support
- **Email**: Contact the development team for enterprise support

## 🔐 Security Notes

- Default passwords are for development only
- Change all default credentials in production
- Use HTTPS in production environments
- Regular security updates are recommended
- Follow GDPR/FERPA compliance guidelines

---

Happy proctoring! 🎓✨