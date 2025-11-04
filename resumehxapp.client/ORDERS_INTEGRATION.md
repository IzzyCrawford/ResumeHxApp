# Order Processing Frontend Integration

## Overview

The Order Processing System is now integrated into the ResumeHxApp frontend with a complete UI for creating orders and tracking their status in real-time.

## Features

### 1. Create Order Form
- **Customer Information**: Enter customer ID
- **Shipping Address**: Complete address form with validation
- **Order Items**: Dynamic item list (add/remove items)
  - SKU, name, quantity, unit price
  - Taxable checkbox per item
- **Shipping Cost**: Configurable shipping amount
- **Live Calculation**: Real-time subtotal, tax (9.85%), and total

### 2. Order Status Tracking
- **Real-time Updates**: Auto-polling every 2 seconds
- **Visual Timeline**: Progress indicator showing order flow
- **Status Icons**: Visual status indicators
- **Full Order Details**: View items, shipping address, pricing breakdown

### 3. Order Status Flow

#### Happy Path
1. **Created** ⏳ - Order submitted
2. **Accepted** 🔄 - Order accepted for processing
3. **InventoryReserved** 📦 - Inventory successfully reserved
4. **PaymentAuthorized** 💳 - Payment authorized
5. **Confirmed** ✅ - Order completed successfully

#### Failure States
- **FailedInventory** ❌ - Insufficient inventory (5% chance)
- **FailedPayment** ❌ - Payment declined (10% chance)
- **EmailFailed** ⚠️ - Order confirmed, but email failed (2% chance - non-blocking)
- **Cancelled** 🚫 - Order cancelled by admin
- **Failed** ❌ - General processing failure

## Setup

### Prerequisites
1. Order Processing API running on `http://localhost:5001`
2. Worker service processing orders
3. PostgreSQL and RabbitMQ running

### Start the System

**Terminal 1 - Infrastructure:**
```powershell
cd c:\Users\Izzy-PC\source\repos\ResumeHxApp
docker compose -f docker-compose.dev.yml up -d postgres rabbitmq
```

**Terminal 2 - Order Processing API:**
```powershell
cd c:\Users\Izzy-PC\source\repos\ResumeHxApp\order-processing\src\OrderProcessing.Api
dotnet ef database update
dotnet run
```

**Terminal 3 - Worker:**
```powershell
cd c:\Users\Izzy-PC\source\repos\ResumeHxApp\order-processing\src\OrderProcessing.Worker
dotnet run
```

**Terminal 4 - Frontend:**
```powershell
cd c:\Users\Izzy-PC\source\repos\ResumeHxApp\resumehxapp.client
npm install
npm run dev
```

### Access the Application

- **Frontend**: http://localhost:5173
- **Orders Page**: http://localhost:5173/orders
- **API**: http://localhost:5001
- **RabbitMQ Management**: http://localhost:15672

## Usage

### Creating an Order

1. Navigate to **Orders** in the navigation menu
2. Fill in the form:
   - Customer ID (e.g., "demo-customer")
   - Shipping address details
   - Add one or more items
   - Set shipping cost
3. Review the calculated total
4. Click **Create Order**

### Tracking Order Status

After creating an order:
1. Automatically redirected to Order Status page
2. Watch real-time status updates:
   - Timeline shows progress
   - Status card updates every 2 seconds
   - Terminal states stop polling
3. View complete order details:
   - Customer info and timestamps
   - Shipping address
   - Order items table
   - Pricing breakdown

### Testing Failure Scenarios

The mock providers have built-in failure rates:

```json
{
  "MockProviders": {
    "Pay": { "FailureRate": 0.10 },        // 10% failure
    "Inventory": { "FailureRate": 0.05 },  // 5% failure
    "Email": { "FailureRate": 0.02 }       // 2% failure
  }
}
```

**To test failures**, create multiple orders:
- Approximately 1 in 10 will fail payment authorization
- Approximately 1 in 20 will fail inventory reservation
- Approximately 1 in 50 will have email failures (but still confirm)

### Monitoring

**Watch Worker Terminal** to see processing logs:
```
[12:34:56] Processing OrderCreated for Order abc123...
[12:34:57] Reserving inventory for Order abc123
[12:34:57] Inventory reserved for Order abc123: 2 items
[12:34:58] Authorizing payment for Order abc123
[12:34:59] Payment authorized: IntentId=pi_xyz789
[12:35:00] Sending confirmation email for Order abc123
[12:35:01] Order abc123 processing completed successfully
```

**Check RabbitMQ UI** at http://localhost:15672:
- View queues and message rates
- Monitor consumer activity
- Check dead-letter queue for failed messages

## Architecture

### Frontend Components

```
src/components/Orders/
├── index.ts                 # Public exports
├── types.ts                 # TypeScript type definitions
├── orderApiService.ts       # API client with polling
├── OrdersPage.tsx           # Main container component
├── CreateOrder.tsx          # Order creation form
├── CreateOrder.css          # Form styles
├── OrderStatus.tsx          # Status tracking view
└── OrderStatus.css          # Status page styles
```

### API Integration

**Create Order:**
```typescript
POST http://localhost:5001/api/orders
Headers:
  Content-Type: application/json
  Idempotency-Key: web-{timestamp}-{random}

Body: CreateOrderRequest
Response: OrderResponse (202 Accepted)
```

**Get Order Status:**
```typescript
GET http://localhost:5001/api/orders/{orderId}
Response: OrderDetailResponse
```

**Status Polling:**
- Polls every 2 seconds
- Stops when terminal status reached
- Terminal states: Confirmed, FailedInventory, FailedPayment, Cancelled, Failed

### Data Flow

```
User Input (Form)
    ↓
Create Order API Call
    ↓
Order Saved + OutboxMessage Created
    ↓
202 Accepted Response
    ↓
Navigate to Status Page
    ↓
Poll Order Status (every 2s)
    ↓
Worker Processing (background)
    ↓
Status Updates Reflected in UI
    ↓
Terminal Status → Stop Polling
```

## Styling

### Status Color Coding
- **Pending** (Created): Gray gradient
- **Processing** (Accepted, InventoryReserved, PaymentAuthorized): Purple gradient
- **Success** (Confirmed): Green gradient
- **Failed** (FailedInventory, FailedPayment, Failed): Red gradient
- **Warning** (EmailFailed): Pink gradient
- **Cancelled**: Dark blue gradient

### Responsive Design
- Desktop: Optimized layout with side-by-side cards
- Mobile: Stacked layout for smaller screens
- Timeline adapts from horizontal to vertical on mobile

## Troubleshooting

### Order API Not Responding
```powershell
# Check if API is running
curl http://localhost:5001/api/orders

# Verify in terminal output that API started on port 5001
```

### CORS Errors
The Order Processing API should have CORS enabled. If you see CORS errors, add to `OrderProcessing.Api/Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// After app.Build():
app.UseCors();
```

### Orders Not Processing
1. **Check Worker is running**: Look for consumer logs
2. **Check RabbitMQ**: Verify queues exist and consumers are connected
3. **Check OutboxMessages**: Query database to see if messages are publishing
4. **Review logs**: All three services (API, Worker, RabbitMQ) for errors

### Frontend Not Loading
```powershell
# Clear node_modules and reinstall
cd resumehxapp.client
rm -r node_modules
rm package-lock.json
npm install
npm run dev
```

## Demo Script

1. **Start all services** (API, Worker, Frontend, Infrastructure)
2. **Navigate to Orders page** in browser
3. **Create first order** with default values
4. **Watch status updates** in real-time
5. **Create 5-10 more orders** to see some failures
6. **Monitor Worker logs** to see processing details
7. **Check RabbitMQ UI** to see message flow
8. **Try duplicate order** (use same customer ID + items) - will be rejected by idempotency

## Next Steps

### Potential Enhancements
1. **Order History**: List of all orders for a customer
2. **Admin Panel**: Cancel orders, view all orders, DLQ management
3. **WebSocket Updates**: Real-time push instead of polling
4. **Email Preview**: Show what confirmation email would look like
5. **Multi-Currency**: Support for different currencies
6. **Inventory Display**: Show available inventory before ordering
7. **Payment Method Selection**: Choose different payment providers
8. **Order Search**: Search by order ID, customer, date range
9. **Export Orders**: Download orders as CSV/PDF
10. **Analytics Dashboard**: Order success rates, failure reasons, etc.

## Files Created

```
resumehxapp.client/src/components/Orders/
├── index.ts                    # Module exports
├── types.ts                    # TypeScript interfaces (170 lines)
├── orderApiService.ts          # API service with polling (75 lines)
├── OrdersPage.tsx              # Main page component (25 lines)
├── CreateOrder.tsx             # Order form component (310 lines)
├── CreateOrder.css             # Form styling (150 lines)
├── OrderStatus.tsx             # Status tracking component (280 lines)
└── OrderStatus.css             # Status page styling (330 lines)

Total: ~1,340 lines of frontend code
```

## Testing

### Manual Test Cases

1. **Happy Path**
   - Create order with 1 item
   - Watch progress through all states
   - Verify Confirmed status

2. **Multiple Items**
   - Create order with 3-5 items
   - Mix taxable and non-taxable
   - Verify tax calculation

3. **Failure Scenarios**
   - Create 20 orders quickly
   - Expect ~2 payment failures
   - Expect ~1 inventory failure
   - Verify compensation (inventory released)

4. **Idempotency**
   - Note an order ID
   - Try to create same order (will fail in backend)
   - Verify duplicate prevention

5. **Status Polling**
   - Create order
   - Watch auto-updates every 2 seconds
   - Verify polling stops at terminal state

## Support

For issues or questions:
1. Check Worker logs for processing errors
2. Check API logs for request errors
3. Check browser console for frontend errors
4. Review RabbitMQ management UI for message flow issues
5. Verify all services are running and accessible
