using MassTransit;
using OrderProcessing.Contracts.V1;
using OrderProcessing.Infrastructure.Data;
using OrderProcessing.Infrastructure.Providers;

namespace OrderProcessing.Worker.Consumers;

public class InventoryReserveConsumer : IConsumer<InventoryReserveRequestV1>
{
    private readonly ApplicationDbContext _context;
    private readonly IMockInventoryProvider _inventoryProvider;
    private readonly ILogger<InventoryReserveConsumer> _logger;

    public InventoryReserveConsumer(
        ApplicationDbContext context,
        IMockInventoryProvider inventoryProvider,
        ILogger<InventoryReserveConsumer> logger)
    {
        _context = context;
        _inventoryProvider = inventoryProvider;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InventoryReserveRequestV1> context)
    {
        var message = context.Message;
        _logger.LogInformation(
            "Reserving inventory for Order {OrderId}, Items: {ItemCount}",
            message.OrderId, message.Items.Length);

        try
        {
            var items = message.Items.Select(i => (i.Sku, i.Quantity)).ToArray();
            var (success, reason) = await _inventoryProvider.ReserveAsync(
                message.OrderId,
                items,
                context.CancellationToken);

            // Save reservation record
            foreach (var item in message.Items)
            {
                var reservation = new InventoryReservation
                {
                    OrderId = message.OrderId,
                    Sku = item.Sku,
                    Quantity = item.Quantity,
                    Status = success ? ReservationStatus.Reserved : ReservationStatus.Failed,
                    FailureReason = reason
                };
                _context.InventoryReservations.Add(reservation);
            }

            await _context.SaveChangesAsync(context.CancellationToken);

            // Reply with result
            await context.RespondAsync(new InventoryReserveResultV1(
                OrderId: message.OrderId,
                Success: success,
                Reason: reason,
                CorrelationId: message.CorrelationId
            ));

            _logger.LogInformation(
                "Inventory reservation for Order {OrderId} completed: Success={Success}",
                message.OrderId, success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reserving inventory for Order {OrderId}", message.OrderId);

            await context.RespondAsync(new InventoryReserveResultV1(
                OrderId: message.OrderId,
                Success: false,
                Reason: $"Internal error: {ex.Message}",
                CorrelationId: message.CorrelationId
            ));
        }
    }
}
