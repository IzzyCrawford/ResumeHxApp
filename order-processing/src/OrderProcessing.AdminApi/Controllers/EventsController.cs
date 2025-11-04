using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Infrastructure.Data;

namespace OrderProcessing.AdminApi.Controllers;

[ApiController]
[Route("admin/orders/{orderId:guid}/[controller]")]
public class EventsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EventsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetOrderEvents(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var orderExists = await _context.Orders.AnyAsync(o => o.Id == orderId, cancellationToken);
        
        if (!orderExists)
        {
            return NotFound(new { error = "Order not found" });
        }

        var events = await _context.OrderEvents
            .Where(e => e.OrderId == orderId)
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new
            {
                e.Id,
                e.EventType,
                e.Payload,
                e.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(events);
    }
}
