# Order Processing System - Quick Start Guide

## Prerequisites
- .NET 9 SDK
- Docker Desktop (for PostgreSQL and RabbitMQ)
- PowerShell (Windows)

## Local Development Setup

### 1. Start Infrastructure Services

```powershell
# Start PostgreSQL and RabbitMQ
cd c:\Users\Izzy-PC\source\repos\ResumeHxApp
docker compose -f docker-compose.dev.yml up -d postgres rabbitmq

# Wait for services to be healthy (check with docker ps)
```

### 2. Run Database Migrations

```powershell
cd order-processing\src\OrderProcessing.Api
dotnet ef database update
```

### 3. Start the Services

Open 3 separate PowerShell terminals:

**Terminal 1 - API (Port 5001)**
```powershell
cd c:\Users\Izzy-PC\source\repos\ResumeHxApp\order-processing\src\OrderProcessing.Api
dotnet run
```

**Terminal 2 - Worker (Background Processing)**
```powershell
cd c:\Users\Izzy-PC\source\repos\ResumeHxApp\order-processing\src\OrderProcessing.Worker
dotnet run
```

**Terminal 3 - Admin API (Port 5002)**
```powershell
cd c:\Users\Izzy-PC\source\repos\ResumeHxApp\order-processing\src\OrderProcessing.AdminApi
dotnet run
```

### 4. Test Order Creation

```powershell
# Create an order
$headers = @{
    "Content-Type" = "application/json"
    "Idempotency-Key" = "test-order-$(Get-Random)"
}

$body = @{
    customerId = "cust-001"
    currency = "USD"
    shippingAddress = @{
        name = "John Doe"
        line1 = "123 Main St"
        city = "Springfield"
        state = "IL"
        postalCode = "62701"
        country = "USA"
    }
    shippingCost = 10.00
    items = @(
        @{
            sku = "WIDGET-001"
            name = "Super Widget"
            quantity = 2
            unitPrice = 29.99
            isTaxable = $true
        },
        @{
            sku = "GADGET-002"
            name = "Mega Gadget"
            quantity = 1
            unitPrice = 49.99
            isTaxable = $true
        }
    )
} | ConvertTo-Json -Depth 10

$response = Invoke-RestMethod -Uri "http://localhost:5001/api/orders" -Method Post -Headers $headers -Body $body
$orderId = $response.orderId
Write-Host "Order created: $orderId"

# Get order details
$order = Invoke-RestMethod -Uri "http://localhost:5001/api/orders/$orderId"
Write-Host "Order Status: $($order.status)"
```

### 5. Monitor Processing

**Watch Worker Logs**
The Worker terminal will show logs for:
- Order acceptance
- Inventory reservation
- Payment authorization
- Email sending
- Status updates

**Check RabbitMQ Management UI**
```powershell
start http://localhost:15672
# Login: guest / guest
```

**Use Admin API to Check Order Details**
```powershell
# Get order with full details
$orderId = "your-order-id-here"
Invoke-RestMethod -Uri "http://localhost:5002/admin/orders/$orderId" | ConvertTo-Json -Depth 10

# Get order events (audit trail)
Invoke-RestMethod -Uri "http://localhost:5002/admin/orders/$orderId/events" | ConvertTo-Json -Depth 10

# Get all orders
Invoke-RestMethod -Uri "http://localhost:5002/admin/orders?page=1&pageSize=10" | ConvertTo-Json -Depth 10

# Check system health
Invoke-RestMethod -Uri "http://localhost:5002/admin/health" | ConvertTo-Json -Depth 10
```

### 6. Test Failure Scenarios

The mock providers have configurable failure rates:
- **Payment**: 10% failure rate
- **Inventory**: 5% failure rate
- **Email**: 2% failure rate

Create multiple orders to see failures:

```powershell
# Create 10 orders to see some failures
1..10 | ForEach-Object {
    $headers = @{
        "Content-Type" = "application/json"
        "Idempotency-Key" = "test-order-$_"
    }
    
    $body = @{
        customerId = "cust-$_"
        currency = "USD"
        shippingAddress = @{
            name = "Customer $_"
            line1 = "$_ Main St"
            city = "Springfield"
            state = "IL"
            postalCode = "62701"
            country = "USA"
        }
        shippingCost = 10.00
        items = @(
            @{
                sku = "ITEM-$_"
                name = "Product $_"
                quantity = 1
                unitPrice = 25.00
                isTaxable = $true
            }
        )
    } | ConvertTo-Json -Depth 10
    
    $response = Invoke-RestMethod -Uri "http://localhost:5001/api/orders" -Method Post -Headers $headers -Body $body
    Write-Host "Order $($_): $($response.orderId) - Status: $($response.status)"
    Start-Sleep -Milliseconds 500
}

# Check statistics
Invoke-RestMethod -Uri "http://localhost:5002/admin/health" | ConvertTo-Json -Depth 10
```

### 7. Test Idempotency

```powershell
# Send the same request twice with same idempotency key
$headers = @{
    "Content-Type" = "application/json"
    "Idempotency-Key" = "duplicate-test-123"
}

$body = @{
    customerId = "cust-duplicate"
    currency = "USD"
    shippingAddress = @{
        name = "Duplicate Test"
        line1 = "123 Duplicate St"
        city = "Springfield"
        state = "IL"
        postalCode = "62701"
        country = "USA"
    }
    shippingCost = 5.00
    items = @(@{
        sku = "DUP-001"
        name = "Duplicate Item"
        quantity = 1
        unitPrice = 10.00
        isTaxable = $true
    })
} | ConvertTo-Json -Depth 10

# First request
$response1 = Invoke-RestMethod -Uri "http://localhost:5001/api/orders" -Method Post -Headers $headers -Body $body
Write-Host "First request - Order ID: $($response1.orderId)"

# Second request (should return same order)
$response2 = Invoke-RestMethod -Uri "http://localhost:5001/api/orders" -Method Post -Headers $headers -Body $body
Write-Host "Second request - Order ID: $($response2.orderId)"

# Verify same order ID
if ($response1.orderId -eq $response2.orderId) {
    Write-Host "✓ Idempotency working correctly!" -ForegroundColor Green
} else {
    Write-Host "✗ Idempotency failed!" -ForegroundColor Red
}
```

### 8. Test Order Cancellation

```powershell
# Create an order
$headers = @{
    "Content-Type" = "application/json"
    "Idempotency-Key" = "cancel-test-$(Get-Random)"
}

$body = @{
    customerId = "cust-cancel"
    currency = "USD"
    shippingAddress = @{
        name = "Cancel Test"
        line1 = "123 Cancel St"
        city = "Springfield"
        state = "IL"
        postalCode = "62701"
        country = "USA"
    }
    shippingCost = 5.00
    items = @(@{
        sku = "CANCEL-001"
        name = "Cancel Item"
        quantity = 1
        unitPrice = 15.00
        isTaxable = $true
    })
} | ConvertTo-Json -Depth 10

$response = Invoke-RestMethod -Uri "http://localhost:5001/api/orders" -Method Post -Headers $headers -Body $body
$orderId = $response.orderId
Write-Host "Order created: $orderId"

# Wait a moment, then cancel
Start-Sleep -Seconds 2
Invoke-RestMethod -Uri "http://localhost:5002/admin/orders/$orderId/cancel" -Method Post
Write-Host "Order cancelled"

# Check updated status
$order = Invoke-RestMethod -Uri "http://localhost:5002/admin/orders/$orderId"
Write-Host "Order Status: $($order.status)"
```

## Configuration

### Mock Provider Failure Rates

Edit `appsettings.json` in `OrderProcessing.Worker`:

```json
"MockProviders": {
  "Pay": {
    "FailureRate": 0.10  // 10% failure rate
  },
  "Inventory": {
    "FailureRate": 0.05  // 5% failure rate
  },
  "Email": {
    "FailureRate": 0.02  // 2% failure rate
  }
}
```

### Retry Configuration

Retry policy is configured in Worker `Program.cs`:
- 3 attempts: 5 seconds, 15 seconds, 60 seconds
- After 3 failures, message goes to dead-letter queue

## Architecture Verification

### End-to-End Flow

1. **API** receives POST request → saves Order + OutboxMessage in transaction
2. **OutboxPublisher** polls every 5 seconds → publishes OrderCreatedV1 to RabbitMQ
3. **OrderCreatedConsumer** receives message:
   - Updates Order to `Accepted`
   - Sends `InventoryReserveRequest` → waits for response
   - If success: Updates to `InventoryReserved`
   - Sends `PaymentAuthorizeRequest` → waits for response
   - If success: Updates to `PaymentAuthorized`
   - Sends `EmailSendRequest` → waits for response
   - Updates to `Confirmed` (even if email fails)
4. Each status change creates an `OrderEvent` and publishes `OrderUpdatedV1`

### Database Tables

```sql
-- View orders
SELECT * FROM "Orders" ORDER BY "CreatedAt" DESC;

-- View order events (audit trail)
SELECT * FROM "OrderEvents" WHERE "OrderId" = 'your-order-id' ORDER BY "CreatedAt";

-- View payments
SELECT * FROM "Payments" WHERE "OrderId" = 'your-order-id';

-- View inventory reservations
SELECT * FROM "InventoryReservations" WHERE "OrderId" = 'your-order-id';

-- View outbox messages
SELECT * FROM "OutboxMessages" ORDER BY "CreatedAt" DESC;
```

## Troubleshooting

### Database Connection Issues
```powershell
# Check PostgreSQL is running
docker ps | Select-String postgres

# View PostgreSQL logs
docker logs orderprocessing-postgres
```

### RabbitMQ Issues
```powershell
# Check RabbitMQ is running
docker ps | Select-String rabbitmq

# View RabbitMQ logs
docker logs orderprocessing-rabbitmq
```

### Migration Issues
```powershell
# Drop and recreate database
cd order-processing\src\OrderProcessing.Api
dotnet ef database drop --force
dotnet ef database update
```

### Clear All Data
```powershell
# Stop all services
docker compose -f docker-compose.dev.yml down -v

# Restart fresh
docker compose -f docker-compose.dev.yml up -d postgres rabbitmq
cd order-processing\src\OrderProcessing.Api
dotnet ef database update
```

## Next Steps

- Review logs in all 3 terminals to understand the flow
- Experiment with different failure scenarios
- Check RabbitMQ management UI to see message flow
- Use Admin API to explore order details and events
- Monitor retry behavior by watching Worker logs
- Test compensation logic (inventory release on payment failure)
