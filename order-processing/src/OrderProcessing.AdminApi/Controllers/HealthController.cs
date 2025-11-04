using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Infrastructure.Data;

namespace OrderProcessing.AdminApi.Controllers;

[ApiController]
[Route("admin/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(ApplicationDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> GetHealth(CancellationToken cancellationToken)
    {
        var health = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Database = await CheckDatabaseHealthAsync(cancellationToken),
            Statistics = await GetStatisticsAsync(cancellationToken)
        };

        return Ok(health);
    }

    private async Task<object> CheckDatabaseHealthAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _context.Database.CanConnectAsync(cancellationToken);
            return new { Status = "Healthy", Message = "Database connection successful" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return new { Status = "Unhealthy", Message = ex.Message };
        }
    }

    private async Task<object> GetStatisticsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var totalOrders = await _context.Orders.CountAsync(cancellationToken);
            var ordersByStatus = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToListAsync(cancellationToken);

            var unpublishedMessages = await _context.OutboxMessages
                .CountAsync(m => m.PublishedAt == null, cancellationToken);

            return new
            {
                TotalOrders = totalOrders,
                OrdersByStatus = ordersByStatus,
                UnpublishedOutboxMessages = unpublishedMessages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve statistics");
            return new { Error = ex.Message };
        }
    }
}
