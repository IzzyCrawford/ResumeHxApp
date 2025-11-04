using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Contracts.V1;
using OrderProcessing.Infrastructure.Data;
using OrderProcessing.Infrastructure.Providers;
using System.Text.Json;

namespace OrderProcessing.Worker.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedV1>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly IRequestClient<InventoryReserveRequestV1> _inventoryClient;
    private readonly IRequestClient<PaymentAuthorizeRequestV1> _paymentClient;
    private readonly IRequestClient<EmailSendRequestV1> _emailClient;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderCreatedConsumer(
        ApplicationDbContext context,
        ILogger<OrderCreatedConsumer> logger,
        IRequestClient<InventoryReserveRequestV1> inventoryClient,
        IRequestClient<PaymentAuthorizeRequestV1> paymentClient,
        IRequestClient<EmailSendRequestV1> emailClient,
        IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _logger = logger;
        _inventoryClient = inventoryClient;
        _paymentClient = paymentClient;
        _emailClient = emailClient;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<OrderCreatedV1> context)
    {
        var message = context.Message;
        _logger.LogInformation(
            "Processing OrderCreated for Order {OrderId}, CorrelationId: {CorrelationId}",
            message.OrderId, message.CorrelationId);

        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == message.OrderId, context.CancellationToken);

        if (order == null)
        {
            _logger.LogError("Order {OrderId} not found", message.OrderId);
            return;
        }

        try
        {
            // Update status to Accepted
            await UpdateOrderStatusAsync(order, OrderStatus.Accepted, "Order accepted for processing", context.CancellationToken);

            // Step 1: Reserve Inventory
            _logger.LogInformation("Reserving inventory for Order {OrderId}", order.Id);
            var inventoryRequest = new InventoryReserveRequestV1(
                OrderId: order.Id,
                Items: message.Items.Select(i => new InventoryItemContract(i.Sku, i.Quantity)).ToArray(),
                CorrelationId: message.CorrelationId
            );

            var inventoryResponse = await _inventoryClient.GetResponse<InventoryReserveResultV1>(
                inventoryRequest,
                context.CancellationToken,
                timeout: RequestTimeout.After(s: 30));

            if (!inventoryResponse.Message.Success)
            {
                await UpdateOrderStatusAsync(order, OrderStatus.FailedInventory, 
                    inventoryResponse.Message.Reason ?? "Inventory reservation failed", 
                    context.CancellationToken);
                return;
            }

            await UpdateOrderStatusAsync(order, OrderStatus.InventoryReserved, "Inventory reserved", context.CancellationToken);

            // Step 2: Authorize Payment
            _logger.LogInformation("Authorizing payment for Order {OrderId}", order.Id);
            var paymentRequest = new PaymentAuthorizeRequestV1(
                OrderId: order.Id,
                Amount: order.Total,
                Currency: order.Currency,
                CorrelationId: message.CorrelationId
            );

            var paymentResponse = await _paymentClient.GetResponse<PaymentAuthorizeResultV1>(
                paymentRequest,
                context.CancellationToken,
                timeout: RequestTimeout.After(s: 30));

            if (!paymentResponse.Message.Authorized)
            {
                // Compensate: Release inventory
                _logger.LogWarning("Payment failed for Order {OrderId}, releasing inventory", order.Id);
                // Note: In a production system, you would send a ReleaseInventoryCommand here
                
                await UpdateOrderStatusAsync(order, OrderStatus.FailedPayment,
                    paymentResponse.Message.Reason ?? "Payment authorization failed",
                    context.CancellationToken);
                return;
            }

            await UpdateOrderStatusAsync(order, OrderStatus.PaymentAuthorized, "Payment authorized", context.CancellationToken);

            // Step 3: Send Email (non-blocking - order is still confirmed even if email fails)
            _logger.LogInformation("Sending confirmation email for Order {OrderId}", order.Id);
            try
            {
                var emailRequest = new EmailSendRequestV1(
                    OrderId: order.Id,
                    To: order.CustomerId, // In real scenario, this would be customer email
                    Template: "order-confirmation",
                    Model: new { OrderId = order.Id, Total = order.Total, Currency = order.Currency },
                    CorrelationId: message.CorrelationId
                );

                var emailResponse = await _emailClient.GetResponse<EmailSendResultV1>(
                    emailRequest,
                    context.CancellationToken,
                    timeout: RequestTimeout.After(s: 30));

                if (!emailResponse.Message.Sent)
                {
                    _logger.LogWarning(
                        "Email failed for Order {OrderId}: {Reason}. Order still confirmed.",
                        order.Id, emailResponse.Message.Reason);
                    
                    await UpdateOrderStatusAsync(order, OrderStatus.Confirmed, 
                        $"Order confirmed (email failed: {emailResponse.Message.Reason})", 
                        context.CancellationToken);
                }
                else
                {
                    await UpdateOrderStatusAsync(order, OrderStatus.Confirmed, "Order confirmed", context.CancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Email send failed for Order {OrderId}. Order still confirmed.", order.Id);
                await UpdateOrderStatusAsync(order, OrderStatus.Confirmed, 
                    $"Order confirmed (email error: {ex.Message})", 
                    context.CancellationToken);
            }

            _logger.LogInformation("Order {OrderId} processing completed successfully", order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Order {OrderId}", order.Id);
            await UpdateOrderStatusAsync(order, OrderStatus.Failed, $"Processing error: {ex.Message}", context.CancellationToken);
            throw;
        }
    }

    private async Task UpdateOrderStatusAsync(Order order, OrderStatus status, string reason, CancellationToken cancellationToken)
    {
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        // Create event record
        var orderEvent = new OrderEvent
        {
            OrderId = order.Id,
            EventType = $"StatusChanged:{status}",
            Payload = JsonSerializer.Serialize(new { Status = status.ToString(), Reason = reason, Timestamp = DateTime.UtcNow })
        };
        _context.OrderEvents.Add(orderEvent);

        await _context.SaveChangesAsync(cancellationToken);

        // Publish OrderUpdated event
        await _publishEndpoint.Publish(new OrderUpdatedV1(
            OrderId: order.Id,
            Status: status.ToString(),
            Reason: reason,
            CorrelationId: order.CorrelationId
        ), cancellationToken);

        _logger.LogInformation(
            "Order {OrderId} status updated to {Status}: {Reason}",
            order.Id, status, reason);
    }
}
