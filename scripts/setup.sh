#!/bin/bash

# AI Proctoring System Setup Script
echo "🚀 Setting up AI Proctoring System..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    print_error "Docker is not installed. Please install Docker first."
    exit 1
fi

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null; then
    print_error "Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    print_warning ".NET SDK is not installed. Installing .NET 8.0 SDK..."
    # Add installation commands based on OS
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
        # Ubuntu/Debian
        if command -v apt-get &> /dev/null; then
            wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
            sudo dpkg -i packages-microsoft-prod.deb
            sudo apt-get update
            sudo apt-get install -y dotnet-sdk-8.0
            rm packages-microsoft-prod.deb
        fi
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        # macOS
        if command -v brew &> /dev/null; then
            brew install --cask dotnet-sdk
        else
            print_error "Homebrew not found. Please install .NET 8.0 SDK manually from https://dotnet.microsoft.com/download"
            exit 1
        fi
    fi
fi

print_status "Creating necessary directories..."
mkdir -p logs
mkdir -p docker/nginx
mkdir -p src/Services/AI/models
mkdir -p src/Client/Resources/Raw

print_status "Building .NET solutions..."
if [ -f "AIProctoring.sln" ]; then
    dotnet restore AIProctoring.sln
    if [ $? -eq 0 ]; then
        print_success ".NET solution restored successfully"
    else
        print_error "Failed to restore .NET solution"
        exit 1
    fi
else
    print_warning "Solution file not found, building individual projects..."
    # Build shared library first
    if [ -d "src/Shared" ]; then
        print_status "Building shared library..."
        dotnet build src/Shared/AIProctoring.Shared.csproj
    fi
    
    # Build services
    for service in Authentication Proctoring Analytics Notifications; do
        if [ -d "src/Services/$service" ]; then
            print_status "Building $service service..."
            dotnet build "src/Services/$service/AIProctoring.$service.csproj"
        fi
    done
    
    # Build client
    if [ -d "src/Client" ]; then
        print_status "Building MAUI client..."
        dotnet build src/Client/AIProctoring.Client.csproj
    fi
fi

print_status "Creating Docker network..."
docker network create aiproctoring-network 2>/dev/null || true

print_status "Creating required configuration files..."

# Create Nginx configuration
cat > docker/nginx/nginx.conf << 'EOF'
events {
    worker_connections 1024;
}

http {
    upstream auth-service {
        server auth-service:8080;
    }
    
    upstream proctoring-service {
        server proctoring-service:8080;
    }
    
    upstream analytics-service {
        server analytics-service:8080;
    }
    
    upstream notifications-service {
        server notifications-service:8080;
    }
    
    upstream ai-service {
        server ai-service:8000;
    }

    server {
        listen 80;
        server_name localhost;

        # API Gateway routing
        location /api/auth/ {
            proxy_pass http://auth-service/api/auth/;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }

        location /api/proctoring/ {
            proxy_pass http://proctoring-service/api/proctoring/;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }

        location /api/analytics/ {
            proxy_pass http://analytics-service/api/analytics/;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }

        location /api/notifications/ {
            proxy_pass http://notifications-service/api/notifications/;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }

        location /api/ai/ {
            proxy_pass http://ai-service/;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }

        # WebSocket support for SignalR
        location /hubs/ {
            proxy_pass http://notifications-service/hubs/;
            proxy_http_version 1.1;
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection "upgrade";
            proxy_set_header Host $host;
            proxy_cache_bypass $http_upgrade;
        }
    }
}
EOF

# Create OpenCV cascade file (mock for now)
print_status "Setting up OpenCV resources..."
if [ ! -f "src/Client/Resources/Raw/haarcascade_frontalface_alt.xml" ]; then
    print_warning "OpenCV cascade file not found. Downloading..."
    curl -L -o "src/Client/Resources/Raw/haarcascade_frontalface_alt.xml" \
        "https://raw.githubusercontent.com/opencv/opencv/master/data/haarcascades/haarcascade_frontalface_alt.xml"
fi

print_status "Starting database services..."
docker-compose up -d postgres redis

print_status "Waiting for database to be ready..."
sleep 10

print_status "Building and starting all services..."
docker-compose up -d --build

print_status "Waiting for services to start..."
sleep 30

print_status "Checking service health..."
services=("auth-service:7001" "proctoring-service:7002" "analytics-service:7003" "notifications-service:7004")

for service in "${services[@]}"; do
    IFS=':' read -r name port <<< "$service"
    if curl -f -s "http://localhost:$port/health" > /dev/null 2>&1; then
        print_success "$name is healthy"
    else
        print_warning "$name may not be ready yet"
    fi
done

print_success "🎉 AI Proctoring System setup complete!"
echo ""
echo "📋 Service URLs:"
echo "   • Authentication Service: http://localhost:7001"
echo "   • Proctoring Service: http://localhost:7002"
echo "   • Analytics Service: http://localhost:7003"
echo "   • Notifications Service: http://localhost:7004"
echo "   • AI Service: http://localhost:8000"
echo "   • API Gateway: http://localhost:80"
echo ""
echo "📊 Management URLs:"
echo "   • PostgreSQL: localhost:5432 (user: aiproctoring, password: aiproctoring123)"
echo "   • Redis: localhost:6379"
echo ""
echo "🔧 To build and run the MAUI client:"
echo "   cd src/Client"
echo "   dotnet build"
echo "   dotnet run"
echo ""
echo "📖 View logs: docker-compose logs -f [service-name]"
echo "🛑 Stop services: docker-compose down"
echo "🔄 Restart services: docker-compose restart"