# ResumeHx

A modern, full-stack resume management application built with React, .NET 9, and PostgreSQL, fully containerized with Docker.

## 🚀 Features

- **React Frontend** - Modern, responsive UI built with React 18 and TypeScript
- **.NET 9 Backend** - High-performance Web API with OpenAPI/Swagger documentation
- **PostgreSQL Database** - Robust data persistence with Entity Framework Core
- **Docker Support** - Fully containerized application with Docker Compose
- **RESTful API** - Complete CRUD operations for resume management
- **Database Seeding** - Pre-configured sample data for quick testing
- **CORS Enabled** - Secure cross-origin resource sharing configuration
- **Hot Reload** - Vite-powered development with instant feedback

## 📋 Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (includes Docker Compose)
- [Node.js 20+](https://nodejs.org/) (for local development)
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (for local development)

## 🏃 Quick Start

### Using Docker (Recommended)

1. **Clone the repository**
   ```bash
   git clone https://github.com/IzzyCrawford/ResumeHxApp.git
   cd ResumeHxApp
   ```

2. **Start the application**
   ```bash
   docker compose up -d
   ```

3. **Access the application**
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5000
   - API Documentation: http://localhost:5000/openapi/v1.json

4. **Stop the application**
   ```bash
   docker compose down
   ```

### Local Development

#### Backend (.NET)

```bash
cd ResumeHxApp.Server
dotnet restore
dotnet run
```

#### Frontend (React)

```bash
cd resumehxapp.client
npm install
npm run dev
```

## 🏗️ Architecture

### Technology Stack

- **Frontend**
  - React 18
  - TypeScript
  - Vite
  - React Router DOM
  - CSS Modules

- **Backend**
  - .NET 9
  - ASP.NET Core Web API
  - Entity Framework Core 9
  - Npgsql (PostgreSQL provider)
  - OpenAPI/Swagger

- **Database**
  - PostgreSQL 16 (Alpine)

- **Infrastructure**
  - Docker & Docker Compose
  - Nginx (for frontend static hosting)

### Project Structure

```
ResumeHxApp/
├── resumehxapp.client/          # React frontend application
│   ├── src/
│   │   ├── components/          # React components
│   │   │   ├── Home/           # Home page
│   │   │   ├── Forecast/       # Weather forecast demo
│   │   │   └── Navigation/     # Navigation bar
│   │   ├── App.tsx             # Main application component
│   │   └── main.tsx            # Application entry point
│   ├── Dockerfile              # Frontend container configuration
│   └── package.json            # Node dependencies
│
├── ResumeHxApp.Server/         # .NET backend application
│   ├── Controllers/            # API controllers
│   │   ├── ResumesController.cs
│   │   └── WeatherForecastController.cs
│   ├── Data/                   # Data layer
│   │   ├── ApplicationDbContext.cs
│   │   ├── Resume.cs
│   │   ├── JobHistory.cs
│   │   └── DbSeeder.cs
│   ├── Migrations/             # EF Core migrations
│   ├── Dockerfile              # Backend container configuration
│   └── Program.cs              # Application startup
│
└── docker-compose.yml          # Multi-container orchestration
```

## 🗄️ Database Schema

### Resume
- Id (Primary Key)
- FullName
- Email
- Phone
- Summary
- LinkedInProfile
- CreatedAt
- UpdatedAt

### JobHistory
- Id (Primary Key)
- ResumeId (Foreign Key)
- CompanyName
- Location
- JobTitle
- TechStack
- Summary
- StartDate
- EndDate
- CreatedAt
- UpdatedAt

## 🔌 API Endpoints

### Resumes

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/resumes` | Get all resumes |
| GET | `/api/resumes/{id}` | Get resume by ID |
| POST | `/api/resumes` | Create new resume |
| PUT | `/api/resumes/{id}` | Update existing resume |
| DELETE | `/api/resumes/{id}` | Delete resume |

### Weather Forecast (Demo)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/weatherforecast` | Get weather forecast data |

## 🔧 Configuration

### Environment Variables

#### Backend
- `ASPNETCORE_ENVIRONMENT` - Application environment (Development/Production)
- `ASPNETCORE_URLS` - API binding URLs
- `CORS_ORIGINS` - Allowed CORS origins (semicolon-separated)
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string

#### Frontend
- `VITE_API_HOST` - Backend API host URL

### Docker Compose Services

- **postgres** - PostgreSQL database (ephemeral)
- **backend** - .NET API server (port 5000)
- **frontend** - React application (port 3000)

## 🔐 Security Features

- CORS policy enforcement
- Environment-based configuration
- Secure database connections
- Input validation via Entity Framework

## 🧪 Testing

Access the API documentation at http://localhost:5000/openapi/v1.json to test endpoints interactively.

Sample data is automatically seeded on application startup for testing purposes.

## 🐳 Docker Commands

```bash
# Build and start all services
docker compose up -d --build

# View logs
docker compose logs -f

# View specific service logs
docker compose logs -f backend
docker compose logs -f frontend

# Stop all services
docker compose down

# Stop and remove volumes (reset database)
docker compose down -v

# Rebuild specific service
docker compose up -d --build backend
```

## 📝 Development Notes

- The database is configured as **ephemeral** in development mode, meaning data is not persisted between container restarts
- `EnsureCreated()` is used in Development to create the schema without migrations
- Auto-migration runs in Production environments
- Database seeding only occurs if no data exists

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

## 👤 Author

**Israel Crawford**
- GitHub: [@IzzyCrawford](https://github.com/IzzyCrawford)
- LinkedIn: [israel-crawford-b9874960](https://linkedin.com/in/israel-crawford-b9874960)
- Email: israel.crawford2011@gmail.com

## 🙏 Acknowledgments

- Built with modern web technologies and best practices
- Inspired by real-world enterprise application patterns
- Designed for scalability and maintainability

---

⭐ Star this repository if you find it helpful!
