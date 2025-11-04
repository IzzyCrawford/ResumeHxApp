using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Api.Models;
using OrderProcessing.Contracts.V1;
using OrderProcessing.Infrastructure.Data;
using System.Text.Json;

namespace OrderProcessing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrdersController> _logger;
    private const decimal TAX_RATE = 0.0985m;

    public OrdersController(ApplicationDbContext context, ILogger<OrdersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder(
        [FromBody] CreateOrderRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return BadRequest(new { error = "Idempotency-Key header is required" });
        }

        // Check for existing order with same idempotency key
        var existingOrder = await _context.Orders
            .FirstOrDefaultAsync(o => o.IdempotencyKey == idempotencyKey && o.CustomerId == request.CustomerId, cancellationToken);

        if (existingOrder != null)
        {
            _logger.LogInformation("Duplicate request detected for IdempotencyKey: {IdempotencyKey}", idempotencyKey);
            return Accepted(MapToOrderResponse(existingOrder));
        }

        // Calculate amounts
        var subtotal = request.Items.Sum(i => i.UnitPrice * i.Quantity);
        var taxableAmount = request.Items.Where(i => i.IsTaxable).Sum(i => i.UnitPrice * i.Quantity) + request.ShippingCost;
        var tax = Math.Round(taxableAmount * TAX_RATE, 2, MidpointRounding.ToEven);
        var total = subtotal + request.ShippingCost + tax;

        var correlationId = Guid.NewGuid().ToString();
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Currency = request.Currency,
            IdempotencyKey = idempotencyKey,
            CorrelationId = correlationId,
            Status = OrderStatus.Created,
            ShippingName = request.ShippingAddress.Name,
            ShippingLine1 = request.ShippingAddress.Line1,
            ShippingLine2 = request.ShippingAddress.Line2,
            ShippingCity = request.ShippingAddress.City,
            ShippingState = request.ShippingAddress.State,
            ShippingPostalCode = request.ShippingAddress.PostalCode,
            ShippingCountry = request.ShippingAddress.Country,
            ShippingCost = request.ShippingCost,
            Subtotal = subtotal,
            Tax = tax,
            Total = total,
            TaxRate = TAX_RATE
        };

        // Add order items
        foreach (var item in request.Items)
        {
            order.Items.Add(new OrderItem
            {
                Sku = item.Sku,
                Name = item.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                IsTaxable = item.IsTaxable
            });
        }

        // Create outbox message
        var orderCreatedMessage = new OrderCreatedV1(
            OrderId: order.Id,
            CustomerId: order.CustomerId,
            Currency: order.Currency,
            Items: request.Items.Select(i => new OrderItemContract(i.Sku, i.Name, i.Quantity, i.UnitPrice, i.IsTaxable)).ToArray(),
            Subtotal: subtotal,
            ShippingCost: request.ShippingCost,
            Tax: tax,
            Total: total,
            CorrelationId: correlationId
        );

        var outboxMessage = new OutboxMessage
        {
            Type = nameof(OrderCreatedV1),
            Payload = JsonSerializer.Serialize(orderCreatedMessage),
            CorrelationId = correlationId
        };

        // Save order + outbox message in single transaction
        _context.Orders.Add(order);
        _context.OutboxMessages.Add(outboxMessage);
        
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order created: {OrderId} with CorrelationId: {CorrelationId}", order.Id, correlationId);

        return Accepted(MapToOrderResponse(order));
    }

    [HttpGet("{orderId:guid}")]
    public async Task<ActionResult<OrderDetailResponse>> GetOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(new OrderDetailResponse(
            OrderId: order.Id,
            Status: order.Status.ToString(),
            CustomerId: order.CustomerId,
            Currency: order.Currency,
            ShippingAddress: new ShippingAddressRequest(
                order.ShippingName,
                order.ShippingLine1,
                order.ShippingLine2,
                order.ShippingCity,
                order.ShippingState,
                order.ShippingPostalCode,
                order.ShippingCountry
            ),
            Subtotal: order.Subtotal,
            ShippingCost: order.ShippingCost,
            Tax: order.Tax,
            Total: order.Total,
            CreatedAt: order.CreatedAt,
            UpdatedAt: order.UpdatedAt,
            Items: order.Items.Select(i => new OrderItemResponse(
                i.Sku, i.Name, i.Quantity, i.UnitPrice, i.IsTaxable
            )).ToArray()
        ));
    }

    private static OrderResponse MapToOrderResponse(Order order)
    {
        return new OrderResponse(
            OrderId: order.Id,
            Status: order.Status.ToString(),
            CustomerId: order.CustomerId,
            Currency: order.Currency,
            Subtotal: order.Subtotal,
            ShippingCost: order.ShippingCost,
            Tax: order.Tax,
            Total: order.Total,
            CreatedAt: order.CreatedAt
        );
    }
}
