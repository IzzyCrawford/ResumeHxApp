using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Contracts.V1;
using OrderProcessing.Infrastructure.Data;
using System.Text.Json;

namespace OrderProcessing.Api.Services;

public class OutboxPublisher : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxPublisher> _logger;
    private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(5);

    public OutboxPublisher(IServiceProvider serviceProvider, ILogger<OutboxPublisher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Publisher started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessages(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages");
            }

            await Task.Delay(_pollInterval, stoppingToken);
        }

        _logger.LogInformation("Outbox Publisher stopped");
    }

    private async Task ProcessOutboxMessages(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        var unpublishedMessages = await context.OutboxMessages
            .Where(m => m.PublishedAt == null)
            .OrderBy(m => m.CreatedAt)
            .Take(100)
            .ToListAsync(cancellationToken);

        foreach (var message in unpublishedMessages)
        {
            try
            {
                await PublishMessage(publishEndpoint, message, cancellationToken);

                message.PublishedAt = DateTime.UtcNow;
                message.Attempts++;
                await context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Published outbox message: {MessageId} Type: {Type}", message.Id, message.Type);
            }
            catch (Exception ex)
            {
                message.Attempts++;
                message.ErrorMessage = ex.Message;
                await context.SaveChangesAsync(cancellationToken);

                _logger.LogError(ex, "Failed to publish outbox message: {MessageId}", message.Id);
            }
        }
    }

    private async Task PublishMessage(IPublishEndpoint publishEndpoint, OutboxMessage message, CancellationToken cancellationToken)
    {
        switch (message.Type)
        {
            case nameof(OrderCreatedV1):
                var orderCreated = JsonSerializer.Deserialize<OrderCreatedV1>(message.Payload);
                if (orderCreated != null)
                {
                    await publishEndpoint.Publish(orderCreated, cancellationToken);
                }
                break;

            case nameof(OrderUpdatedV1):
                var orderUpdated = JsonSerializer.Deserialize<OrderUpdatedV1>(message.Payload);
                if (orderUpdated != null)
                {
                    await publishEndpoint.Publish(orderUpdated, cancellationToken);
                }
                break;

            default:
                _logger.LogWarning("Unknown message type: {Type}", message.Type);
                break;
        }
    }
}
