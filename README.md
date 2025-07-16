# AI-Proctored Online Exam System

A comprehensive cross-platform AI-powered proctoring solution for online examinations with real-time monitoring, behavior analysis, and privacy-first design.

## Features

- **Multi-modal Detection**: Video, audio, screen, and behavior monitoring
- **Cross-platform Client**: MAUI application for Windows, macOS, iOS, and Android
- **Real-time Processing**: Live monitoring with cloud-based AI analysis
- **Privacy-by-Design**: Data minimization, consent management, and GDPR compliance
- **Adaptive Monitoring**: Adjusts sensitivity based on exam type and student needs
- **Anti-tampering**: Advanced security measures to prevent cheating

## Architecture

### Frontend (MAUI Client)
- Cross-platform exam taking interface
- Local face detection and basic monitoring
- Real-time communication with backend services
- Offline capability with sync when connection resumes

### Backend Services
- **Authentication Service**: Identity management and JWT tokens
- **Proctoring Engine**: Core monitoring and violation detection
- **AI Inference Service**: Machine learning models for behavior analysis
- **Notification Service**: Real-time alerts and communications
- **Analytics Service**: Time-series data analysis and reporting

### Technology Stack
- **Frontend**: .NET MAUI, OpenCvSharp, ONNX Runtime
- **Backend**: ASP.NET Core, SignalR, Entity Framework
- **AI/ML**: ONNX models, Python FastAPI for advanced inference
- **Database**: PostgreSQL with TimescaleDB for time-series data
- **Infrastructure**: Docker, Azure/AWS cloud services

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- Docker (for backend services)
- Python 3.8+ (for AI services)

### Quick Start
1. Clone the repository
2. Run `docker-compose up` for backend services
3. Open the MAUI solution in Visual Studio
4. Build and run the client application

## Privacy & Ethics

This system is built with privacy-by-design principles:
- Explicit consent for all monitoring features
- Data minimization and automatic deletion
- Transparent AI with explainable decisions
- Accessibility compliance (WCAG 2.1 AA)
- Full GDPR and FERPA compliance

## License

MIT License - see LICENSE file for details.