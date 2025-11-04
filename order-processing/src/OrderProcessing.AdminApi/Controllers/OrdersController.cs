using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Infrastructure.Data;

namespace OrderProcessing.AdminApi.Controllers;

[ApiController]
[Route("admin/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(ApplicationDbContext context, ILogger<OrdersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> GetOrders(
        [FromQuery] string? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Orders.AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var orderStatus))
        {
            query = query.Where(o => o.Status == orderStatus);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= toDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new
            {
                o.Id,
                o.CustomerId,
                o.Status,
                o.Currency,
                o.Total,
                o.CreatedAt,
                o.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Orders = orders
        });
    }

    [HttpGet("{orderId:guid}")]
    public async Task<ActionResult> GetOrderDetails(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Payments)
            .Include(o => o.InventoryReservations)
            .Include(o => o.Events.OrderByDescending(e => e.CreatedAt))
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            order.Id,
            order.CustomerId,
            order.Status,
            order.Currency,
            order.IdempotencyKey,
            order.CorrelationId,
            order.Subtotal,
            order.ShippingCost,
            order.Tax,
            order.Total,
            order.TaxRate,
            ShippingAddress = new
            {
                order.ShippingName,
                order.ShippingLine1,
                order.ShippingLine2,
                order.ShippingCity,
                order.ShippingState,
                order.ShippingPostalCode,
                order.ShippingCountry
            },
            Items = order.Items.Select(i => new
            {
                i.Sku,
                i.Name,
                i.Quantity,
                i.UnitPrice,
                i.IsTaxable
            }),
            Payments = order.Payments.Select(p => new
            {
                p.Id,
                p.Provider,
                p.IntentId,
                p.Status,
                p.Amount,
                p.AuthorizedAt,
                p.FailureReason,
                p.CreatedAt
            }),
            InventoryReservations = order.InventoryReservations.Select(r => new
            {
                r.Id,
                r.Sku,
                r.Quantity,
                r.Status,
                r.ReservedAt,
                r.ReleasedAt,
                r.FailureReason
            }),
            Events = order.Events.Select(e => new
            {
                e.Id,
                e.EventType,
                e.Payload,
                e.CreatedAt
            }),
            order.CreatedAt,
            order.UpdatedAt
        });
    }

    [HttpPost("{orderId:guid}/cancel")]
    public async Task<ActionResult> CancelOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.InventoryReservations)
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order == null)
        {
            return NotFound();
        }

        // Can only cancel if not yet confirmed
        if (order.Status == OrderStatus.Confirmed)
        {
            return BadRequest(new { error = "Cannot cancel a confirmed order" });
        }

        _logger.LogInformation("Cancelling order {OrderId}", orderId);

        // Release inventory reservations
        foreach (var reservation in order.InventoryReservations.Where(r => r.Status == ReservationStatus.Reserved))
        {
            reservation.Status = ReservationStatus.Released;
            reservation.ReleasedAt = DateTime.UtcNow;
        }

        // Update order status
        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        // Add event
        var orderEvent = new OrderEvent
        {
            OrderId = order.Id,
            EventType = "StatusChanged:Cancelled",
            Payload = System.Text.Json.JsonSerializer.Serialize(new 
            { 
                Status = "Cancelled", 
                Reason = "Admin cancelled", 
                Timestamp = DateTime.UtcNow 
            })
        };
        _context.OrderEvents.Add(orderEvent);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} cancelled successfully", orderId);

        return Ok(new { message = "Order cancelled successfully" });
    }
}
