# ResumeHx

A modern, full-stack application showcasing enterprise patterns with React, .NET 9, PostgreSQL, and event-driven architecture using MassTransit and RabbitMQ. Fully containerized with Docker.

## 🚀 Features

### Core Application
- **React Frontend** - Modern, responsive UI built with React 18 and TypeScript
- **.NET 9 Backend** - High-performance Web API with OpenAPI/Swagger documentation
- **PostgreSQL Database** - Robust data persistence with Entity Framework Core
- **Docker Support** - Fully containerized application with single Docker Compose file
- **RESTful API** - Complete CRUD operations for resume management
- **CORS Enabled** - Secure cross-origin resource sharing configuration
- **Hot Reload** - Vite-powered development with instant feedback

### Order Processing System (Demo)
- **Event-Driven Architecture** - MassTransit with RabbitMQ for distributed messaging
- **Transactional Outbox Pattern** - Ensures reliable message delivery
- **Idempotency** - Duplicate requests safely handled
- **Saga Orchestration** - Multi-step order processing workflow (Inventory → Payment → Email)
- **Admin API** - Health checks, statistics, and order management
- **Mock Providers** - Simulated payment, inventory, and email services
- **Retry Logic** - Automatic retry with dead-letter queue for failed messages

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

2. **Start all services**
   ```bash
   docker compose up -d --build
   ```
   This starts:
   - PostgreSQL database
   - RabbitMQ message broker
   - Order Processing API, Worker, and Admin API
   - ResumeHx backend API
   - React frontend (nginx)

3. **Access the application**
   - **Frontend:** http://localhost:3000
   - **ResumeHx API:** http://localhost:5083
   - **Orders API:** http://localhost:5001
   - **Admin API:** http://localhost:5002
   - **RabbitMQ Management:** http://localhost:15672 (guest/guest)

4. **Test the order system**
   ```powershell
   # Run the end-to-end test script
   pwsh .\order-processing\test-order.ps1
   ```

5. **Stop the application**
   ```bash
   docker compose down
   ```

6. **Stop and reset (removes volumes)**
   ```bash
   docker compose down -v
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
  - Modern responsive CSS with Grid/Flexbox

- **Backend**
  - .NET 9
  - ASP.NET Core Web API
  - Entity Framework Core 9
  - Npgsql (PostgreSQL provider)
  - OpenAPI/Swagger

- **Messaging & Events**
  - MassTransit 8.5
  - RabbitMQ 4.0 (with Management UI)
  - Transactional Outbox Pattern
  - Event-driven saga orchestration

- **Database**
  - PostgreSQL 17 (Alpine)
  - Two databases: `orderprocessing`, `resumehxdb`

- **Infrastructure**
  - Docker & Docker Compose (unified single-file)
  - Nginx (for frontend static hosting)
  - Multi-service orchestration with health checks

### Project Structure

```
ResumeHxApp/
├── resumehxapp.client/          # React frontend (SPA)
│   ├── src/
│   │   ├── components/
│   │   │   ├── Home/           # Home page
│   │   │   ├── Resume/         # Resume management
│   │   │   ├── Forecast/       # Weather forecast demo
│   │   │   ├── Orders/         # Order processing UI
│   │   │   │   ├── CreateOrder.tsx
│   │   │   │   ├── OrderStatus.tsx
│   │   │   │   ├── OrdersPage.tsx
│   │   │   │   └── orderApiService.ts
│   │   │   └── Navigation/     # Navigation bar
│   │   ├── App.tsx             # Main application component
│   │   └── main.tsx            # Application entry point
│   ├── Dockerfile              # Nginx production build
│   └── package.json
│
├── ResumeHxApp.Server/         # .NET backend (Resume API)
│   ├── Controllers/
│   │   ├── ResumesController.cs
│   │   └── WeatherForecastController.cs
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Resume.cs
│   │   ├── JobHistory.cs
│   │   └── DbSeeder.cs
│   ├── Migrations/
│   ├── Dockerfile
│   └── Program.cs
│
├── order-processing/            # Event-driven order system
│   ├── src/
│   │   ├── OrderProcessing.Api/              # REST API
│   │   │   ├── Controllers/OrdersController.cs
│   │   │   └── Program.cs
│   │   ├── OrderProcessing.Worker/           # MassTransit consumers
│   │   │   ├── Consumers/
│   │   │   │   ├── OrderCreatedConsumer.cs
│   │   │   │   ├── InventoryReserveConsumer.cs
│   │   │   │   ├── PaymentAuthorizeConsumer.cs
│   │   │   │   └── EmailSendConsumer.cs
│   │   │   └── Program.cs
│   │   ├── OrderProcessing.AdminApi/         # Admin endpoints
│   │   │   ├── Controllers/
│   │   │   │   ├── HealthController.cs
│   │   │   │   └── StatsController.cs
│   │   │   └── Program.cs
│   │   ├── OrderProcessing.Contracts/        # Message contracts
│   │   └── OrderProcessing.Infrastructure/   # Data, Outbox, Providers
│   │       ├── Data/
│   │       │   ├── Order.cs
│   │       │   ├── OrderItem.cs
│   │       │   ├── Payment.cs
│   │       │   ├── InventoryReservation.cs
│   │       │   └── OutboxMessage.cs
│   │       ├── Outbox/OutboxPublisher.cs
│   │       └── Providers/
│   │           ├── MockPayProvider.cs
│   │           ├── MockInventoryProvider.cs
│   │           └── MockEmailProvider.cs
│   ├── test-order.ps1           # E2E test script
│   └── README.md                # Detailed order system docs
│
├── db-init/
│   └── 01-init.sql              # Database initialization
│
└── docker-compose.yml           # Unified multi-service stack
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

### ResumeHx API (Port 5083)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/resumes` | Get all resumes |
| GET | `/api/resumes/{id}` | Get resume by ID |
| POST | `/api/resumes` | Create new resume |
| PUT | `/api/resumes/{id}` | Update existing resume |
| DELETE | `/api/resumes/{id}` | Delete resume |
| GET | `/weatherforecast` | Weather forecast demo |

### Orders API (Port 5001)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/orders` | Create new order (idempotent) |
| GET | `/api/orders/{id}` | Get order details |
| GET | `/api/orders` | List all orders |

**Headers:**
- `Idempotency-Key` - Required for POST (prevents duplicate orders)

### Admin API (Port 5002)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | System health check |
| GET | `/stats` | Order statistics by status |
| GET | `/payments` | List all payments |
| GET | `/inventory` | List inventory reservations |
| GET | `/events` | List order events |

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

- **postgres** - PostgreSQL 17 database (persisted via volume)
- **rabbitmq** - RabbitMQ 4.0 with management UI (ports 5672, 15672)
- **api** - Order Processing API (port 5001)
- **worker** - Order Processing Worker (background consumer)
- **adminapi** - Order Admin API (port 5002)
- **backend** - ResumeHx API server (port 5083)
- **frontend** - React SPA via Nginx (port 3000)

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

# View logs for all services
docker compose logs -f

# View specific service logs
docker compose logs -f api        # Order API
docker compose logs -f worker     # Order Worker
docker compose logs -f backend    # Resume backend
docker compose logs -f frontend   # React SPA

# Check service status
docker compose ps

# Stop all services
docker compose down

# Stop and remove volumes (reset databases)
docker compose down -v

# Rebuild specific service
docker compose up -d --build api
docker compose up -d --build frontend

# Restart a service
docker compose restart worker
```

## 🧪 Testing the Order System

### Using PowerShell Script (Recommended)

```powershell
# Run the automated end-to-end test
pwsh .\order-processing\test-order.ps1
```

This script will:
1. Create a test order with idempotency
2. Verify idempotency (duplicate request)
3. Wait for worker processing
4. Check order status transitions
5. Verify Admin API health and statistics

### Manual Testing with cURL

```bash
# Create an order
curl -X POST http://localhost:5001/api/orders \
  -H "Content-Type: application/json" \
  -H "Idempotency-Key: test-123" \
  -d '{
    "customerId": "demo-customer",
    "currency": "USD",
    "shippingAddress": {
      "name": "John Doe",
      "line1": "123 Main St",
      "city": "Seattle",
      "state": "WA",
      "postalCode": "98101",
      "country": "USA"
    },
    "shippingCost": 15.00,
    "items": [
      {
        "sku": "WIDGET-001",
        "name": "Super Widget",
        "quantity": 2,
        "unitPrice": 29.99,
        "isTaxable": true
      }
    ]
  }'

# Get order status
curl http://localhost:5001/api/orders/{orderId}

# Check system health
curl http://localhost:5002/health

# View statistics
curl http://localhost:5002/stats
```

### Using the Frontend

1. Navigate to http://localhost:3000/orders
2. Fill out the order form
3. Click "Create Order"
4. Watch the order status update in real-time
5. View processing stages: Created → Accepted → Inventory Reserved → Payment Authorized → Confirmed

## 📝 Development Notes

### Database
- PostgreSQL data is **persisted** via Docker volume `postgres_data`
- Two databases: `orderprocessing` and `resumehxdb`
- Auto-migrations run at startup for all services
- Resume database seeding occurs if no data exists

### Order Processing
- **Transactional Outbox:** Messages are stored in the database and published asynchronously
- **Idempotency:** Use `Idempotency-Key` header to prevent duplicate orders
- **Retry Logic:** Failed messages are retried 3 times before moving to dead-letter queue
- **Mock Providers:** Payment, inventory, and email services are simulated for demo purposes
- **Worker Processing:** Consumes messages in order: Inventory → Payment → Email

### Frontend
- React SPA with responsive two-column layout on orders page
- Real-time order status polling (updates every 2 seconds)
- Optimized for minimal scrolling with sticky order summary
- Production build served via Nginx

### CORS
- API allows origins: `http://localhost:5173` (Vite dev) and `http://localhost:3000` (production)
- Configured for cross-origin requests from frontend to multiple backend services

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

## 🎯 Key Patterns Demonstrated

### Event-Driven Architecture
- **Message Broker:** RabbitMQ for async communication
- **MassTransit:** .NET integration with transport abstraction
- **Saga Pattern:** Multi-step order workflow with compensation

### Reliability Patterns
- **Transactional Outbox:** Ensures at-least-once message delivery
- **Idempotency:** Prevents duplicate processing
- **Retry & DLQ:** Automatic retry with dead-letter queue for poison messages
- **Circuit Breaker:** Simulated failures in mock providers

### Clean Architecture
- **CQRS-lite:** Separate read/write paths via events
- **Domain Events:** OrderCreated, InventoryReserved, PaymentAuthorized
- **Repository Pattern:** Data access abstraction
- **Dependency Injection:** Service registration and lifecycle management

### DevOps & Observability
- **Docker Compose:** Single-command stack deployment
- **Health Checks:** Database and RabbitMQ readiness probes
- **Structured Logging:** JSON logs for analysis
- **Auto-Migrations:** Zero-downtime database updates

## 📚 Additional Documentation

- [Order Processing System Details](./order-processing/README.md)
- [Implementation Status](./order-processing/IMPLEMENTATION_STATUS.md)
- [Quick Start Guide](./order-processing/QUICK_START.md)
- [Frontend Client](./resumehxapp.client/README.md)

## 🙏 Acknowledgments

- Built with modern web technologies and best practices
- Inspired by real-world enterprise application patterns
- Demonstrates microservices, event sourcing, and CQRS concepts
- Designed for scalability, maintainability, and reliability

---

⭐ Star this repository if you find it helpful!
