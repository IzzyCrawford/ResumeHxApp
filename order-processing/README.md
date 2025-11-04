# Order Processing System

A distributed, event-driven order processing system built with .NET 9, MassTransit, and PostgreSQL, demonstrating production-grade patterns: Outbox, idempotency, retries, DLQ management, and observability.

## 🏗️ Architecture

```
┌─────────────┐
│   Client    │
└──────┬──────┘
       │ POST /api/orders
       ▼
┌─────────────────────────────────────────────────┐
│              API (ASP.NET Core)                  │
│  • Idempotency-Key validation                    │
│  • Save Order + Outbox (single transaction)      │
│  • Return 202 Accepted + OrderId                 │
└──────────────┬──────────────────────────────────┘
               │
               ▼
      ┌────────────────┐
      │   PostgreSQL   │
      │ • Orders       │
      │ • Outbox       │
      │ • Events       │
      └───────┬────────┘
              │
              ▼
       ┌────────────────────┐
       │ Outbox Publisher   │
       │ (HostedService)    │
       └─────────┬──────────┘
                 │ Publishes OrderCreated
                 ▼
        ┌──────────────────┐
        │  Service Bus     │
        │ • RabbitMQ (dev) │
        │ • Azure SB (prod)│
        └────────┬─────────┘
                 │
                 ▼
┌────────────────────────────────────────────────┐
│          Worker (MassTransit Consumers)         │
│  1. Reserve Inventory → Update Order            │
│  2. Authorize Payment → Update Order            │
│  3. Send Email        → Mark Confirmed          │
│  • Retry x3, then DLQ                           │
│  • Emit OrderUpdated events                     │
└─────────────────────────────────────────────────┘
```

## 📋 Features

- **Event-Driven:** MassTransit with RabbitMQ (dev) and Azure Service Bus (prod)
- **Reliability:** Transactional Outbox pattern, retries (3x), dead-letter queue
- **Idempotency:** Duplicate requests return same OrderId
- **Observability:** OpenTelemetry tracing, structured logs, metrics
- **Admin Tools:** List orders, view DLQ, replay messages, cancel orders
- **Tax Calculation:** 9.85% on items + shipping (server-side banker's rounding)
- **Mock Providers:** MockPay, MockInventory, MockEmail with configurable failures

## 🚀 Getting Started

### Prerequisites

- .NET 9 SDK
- Docker Desktop
- Your favorite IDE (VS Code, Rider, Visual Studio)

### Local Development

1. **Start infrastructure:**
   ```bash
   cd order-processing
   docker-compose -f docker-compose.dev.yml up -d postgres rabbitmq
   ```

2. **Run migrations:**
   ```bash
   cd src/OrderProcessing.Api
   dotnet ef database update --project ../OrderProcessing.Infrastructure
   ```

3. **Start services:**
   ```bash
   # Terminal 1: API
   cd src/OrderProcessing.Api
   dotnet run

   # Terminal 2: Worker
   cd src/OrderProcessing.Worker
   dotnet run

   # Terminal 3: Admin API
   cd src/OrderProcessing.AdminApi
   dotnet run
   ```

4. **Test:**
   ```bash
   curl -X POST http://localhost:5000/api/orders \
     -H "Content-Type: application/json" \
     -H "Idempotency-Key: test-123" \
     -d '{
       "customerId": "cust-001",
       "currency": "USD",
       "shippingAddress": {
         "name": "John Doe",
         "line1": "123 Main St",
         "city": "Seattle",
         "state": "WA",
         "postalCode": "98101",
         "country": "USA"
       },
       "shippingCost": 10.00,
       "items": [
         {
           "sku": "WIDGET-01",
           "name": "Super Widget",
           "quantity": 2,
           "unitPrice": 29.99,
           "isTaxable": true
         }
       ]
     }'
   ```

## 📦 Project Structure

```
order-processing/
├── src/
│   ├── OrderProcessing.Api/              # REST API
│   │   ├── Controllers/
│   │   │   └── OrdersController.cs       # POST/GET orders
│   │   ├── DTOs/
│   │   │   ├── CreateOrderRequest.cs
│   │   │   └── OrderResponse.cs
│   │   ├── Middleware/
│   │   │   └── IdempotencyMiddleware.cs
│   │   └── Program.cs
│   │
│   ├── OrderProcessing.Worker/           # MassTransit consumers
│   │   ├── Consumers/
│   │   │   ├── OrderCreatedConsumer.cs
│   │   │   ├── InventoryReserveConsumer.cs
│   │   │   ├── PaymentAuthorizeConsumer.cs
│   │   │   └── EmailSendConsumer.cs
│   │   └── Program.cs
│   │
│   ├── OrderProcessing.Contracts/        # Message contracts
│   │   ├── V1/
│   │   │   ├── OrderCreatedV1.cs
│   │   │   ├── InventoryReserveRequestV1.cs
│   │   │   ├── PaymentAuthorizeRequestV1.cs
│   │   │   └── EmailSendRequestV1.cs
│   │   └── Common/
│   │       └── MessageEnvelope.cs
│   │
│   ├── OrderProcessing.Infrastructure/   # Data, Outbox, Providers
│   │   ├── Data/
│   │   │   ├── Order.cs                  ✅ Created
│   │   │   ├── OrderItem.cs              ✅ Created
│   │   │   ├── OrderStatus.cs            ✅ Created
│   │   │   ├── Payment.cs                🔜 TODO
│   │   │   ├── InventoryReservation.cs   🔜 TODO
│   │   │   ├── OutboxMessage.cs          🔜 TODO
│   │   │   ├── OrderEvent.cs             🔜 TODO
│   │   │   └── ApplicationDbContext.cs   🔜 TODO
│   │   ├── Outbox/
│   │   │   └── OutboxPublisher.cs        🔜 TODO
│   │   ├── Providers/
│   │   │   ├── MockPayProvider.cs        🔜 TODO
│   │   │   ├── MockInventoryProvider.cs  🔜 TODO
│   │   │   └── MockEmailProvider.cs      🔜 TODO
│   │   └── Migrations/
│   │
│   └── OrderProcessing.AdminApi/         # Admin endpoints
│       ├── Controllers/
│       │   ├── AdminOrdersController.cs  🔜 TODO
│       │   └── DlqController.cs          🔜 TODO
│       └── Program.cs
│
├── tests/
│   ├── OrderProcessing.UnitTests/
│   └── OrderProcessing.IntegrationTests/
│
├── docker-compose.dev.yml                🔜 TODO
├── .env.example
└── README.md                             ✅ This file
```

## 🗄️ Data Model

### Order
- `Id` (Guid)
- `CustomerId`, `Status` (enum), `Currency`
- `IdempotencyKey`, `CorrelationId`
- Shipping address fields
- `ShippingCost`, `Subtotal`, `Tax`, `Total`, `TaxRate` (0.0985)
- `CreatedAt`, `UpdatedAt`
- Collections: `Items`, `Payments`, `InventoryReservations`, `Events`

### OrderItem
- `Id`, `OrderId`, `Sku`, `Name`
- `Quantity`, `UnitPrice`, `IsTaxable`

### Payment
- `Id`, `OrderId`, `Provider`, `IntentId`
- `Status` (Authorized|Failed), `Amount`
- `AuthorizedAt`, `FailureReason`

### InventoryReservation
- `Id`, `OrderId`, `Sku`, `Quantity`
- `Status` (Reserved|Released|Failed)
- `ReservedAt`, `ReleasedAt`, `FailureReason`

### OutboxMessage
- `Id`, `Type`, `Payload` (JSON), `CorrelationId`
- `Attempts`, `CreatedAt`, `PublishedAt`

### OrderEvent
- `Id`, `OrderId`, `EventType`, `Payload` (JSON), `CreatedAt`

## 📨 Message Contracts

All messages include: `correlationId`, `schemaVersion`, `occurredAt`

### OrderCreatedV1
```csharp
public record OrderCreatedV1(
    Guid OrderId,
    string CustomerId,
    OrderItemDto[] Items,
    string Currency,
    AmountsDto Amounts
);
```

### InventoryReserveRequestV1
```csharp
public record InventoryReserveRequestV1(Guid OrderId, ReserveItemDto[] Items);
public record InventoryReserveResultV1(Guid OrderId, bool Success, string? Reason);
```

### PaymentAuthorizeRequestV1
```csharp
public record PaymentAuthorizeRequestV1(Guid OrderId, decimal Amount, string Currency);
public record PaymentAuthorizeResultV1(Guid OrderId, bool Authorized, string? IntentId, string? Reason);
```

### EmailSendRequestV1
```csharp
public record EmailSendRequestV1(Guid OrderId, string To, string Template, object Model);
public record EmailSendResultV1(Guid OrderId, bool Sent, string? Reason);
```

## 🔄 Order State Machine

```
Created
  → Accepted (after DB save + Outbox)
    → InventoryReserved (MockInventory success)
      → PaymentAuthorized (MockPay success)
        → Confirmed (email sent, or EmailFailed if retries exhausted)

Failures:
  InventoryReserved → FailedInventory (stop pipeline, emit OrderUpdated)
  PaymentAuthorized → FailedPayment (release inventory, stop pipeline)
  Email retries exhausted → EmailFailed (order stays Confirmed; email non-critical)

Admin Actions:
  Cancel (pre-Confirmed) → release reservation, void payment intent
```

## ⚙️ Configuration

### API (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=orderprocessing;Username=postgres;Password=postgres"
  },
  "MassTransit": {
    "Transport": "RabbitMQ",
    "RabbitMQ": {
      "Host": "rabbitmq://localhost",
      "Username": "guest",
      "Password": "guest"
    },
    "AzureServiceBus": {
      "ConnectionString": "Endpoint=sb://..."
    }
  },
  "OpenTelemetry": {
    "ServiceName": "OrderProcessing.Api",
    "ConsoleExporter": true
  }
}
```

### Worker (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=orderprocessing;Username=postgres;Password=postgres"
  },
  "MassTransit": {
    "Transport": "RabbitMQ",
    "RabbitMQ": {
      "Host": "rabbitmq://localhost",
      "Username": "guest",
      "Password": "guest"
    },
    "RetryPolicy": {
      "MaxRetries": 3,
      "IntervalSeconds": [5, 15, 60]
    }
  },
  "Providers": {
    "MockPay": {
      "FailureRate": 0.1
    },
    "MockInventory": {
      "FailureRate": 0.05
    },
    "MockEmail": {
      "FailureRate": 0.02
    }
  }
}
```

## 🧪 Testing

### Unit Tests
```bash
cd tests/OrderProcessing.UnitTests
dotnet test
```

### Integration Tests (Testcontainers)
```bash
cd tests/OrderProcessing.IntegrationTests
dotnet test
```

## 📊 Observability

- **Logs:** Structured logs with `correlationId` and `orderId`
- **Traces:** OpenTelemetry spans from API → Outbox → Worker
- **Metrics:**
  - API: request rate, latency, 4xx/5xx
  - Queue: depth, consumer lag
  - Worker: processing time, retries, error rate
  - Outbox: pending count, publish latency

### Example trace:
```
API POST /orders [correlationId=abc-123]
  → DB Save Order
  → DB Save Outbox
  → Return 202

Outbox Publisher [correlationId=abc-123]
  → Publish OrderCreated

Worker OrderCreatedConsumer [correlationId=abc-123]
  → Reserve Inventory [orderId=xyz]
  → Authorize Payment
  → Send Email
  → Update Order Status=Confirmed
```

## 🛠️ Admin API

### Endpoints
- `GET /api/admin/orders?status=&page=` - List orders with filters
- `GET /api/admin/orders/{id}/events` - Order timeline
- `POST /api/admin/orders/{id}/cancel` - Cancel pending order
- `GET /api/admin/dlq` - Dead-letter queue entries
- `POST /api/admin/dlq/{messageId}/requeue` - Replay message

## 🐳 Docker Compose

### Services
- **postgres** (persistent DB)
- **rabbitmq** (management UI on 15672)
- **api** (port 5000)
- **worker** (background consumer)
- **admin-api** (port 5001)

```bash
docker-compose -f docker-compose.dev.yml up --build
```

## 🔒 Security Considerations

- **PII:** Encrypt shipping addresses at-rest
- **Payment:** Never store card data; use provider tokens/intents only
- **Idempotency:** Unique index on `(IdempotencyKey, CustomerId)`
- **Auth:** Protect API with API keys or OAuth2 (not shown in v1)

## 🚧 TODO (Next Implementation Steps)

1. **Complete Data Models:**
   - `Payment.cs`
   - `InventoryReservation.cs`
   - `OutboxMessage.cs`
   - `OrderEvent.cs`

2. **Create ApplicationDbContext:**
   - EF configurations for all entities
   - Unique indexes
   - Relationships

3. **Define Message Contracts:**
   - All V1 contracts in `Contracts/V1/`
   - Envelope with correlation

4. **Implement API:**
   - `OrdersController` with POST/GET
   - Idempotency middleware
   - DTOs and validators
   - Transactional outbox insert

5. **Build Outbox Publisher:**
   - Hosted service polling OutboxMessages
   - Publish via MassTransit
   - Mark sent

6. **Build Worker Consumers:**
   - OrderCreatedConsumer orchestrates pipeline
   - InventoryReserveConsumer
   - PaymentAuthorizeConsumer
   - EmailSendConsumer
   - Retry config (3x)

7. **Create Mock Providers:**
   - MockPayProvider (authorize with intentId)
   - MockInventoryProvider (reserve/release)
   - MockEmailProvider (log + simulate)

8. **Admin API:**
   - AdminOrdersController
   - DlqController

9. **Docker Compose:**
   - `docker-compose.dev.yml` with all services

10. **Tests:**
    - Unit tests for validation and state transitions
    - Integration tests with Testcontainers

## 📚 References

- [MassTransit Documentation](https://masstransit.io/)
- [Transactional Outbox Pattern](https://microservices.io/patterns/data/transactional-outbox.html)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/instrumentation/net/)
- [EF Core](https://learn.microsoft.com/en-us/ef/core/)

## 📝 License

MIT

---

**Ready to build a production-grade distributed system?** Follow the TODO list above to complete the implementation. Each step builds on the foundation we've created here.
