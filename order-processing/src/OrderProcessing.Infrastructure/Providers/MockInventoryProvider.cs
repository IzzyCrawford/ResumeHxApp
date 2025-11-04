using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OrderProcessing.Infrastructure.Providers;

public class MockInventoryProvider : IMockInventoryProvider
{
    private readonly ILogger<MockInventoryProvider> _logger;
    private readonly double _failureRate;
    private readonly Random _random = new();

    public MockInventoryProvider(IConfiguration configuration, ILogger<MockInventoryProvider> logger)
    {
        _logger = logger;
        _failureRate = configuration.GetValue<double>("MockProviders:Inventory:FailureRate", 0.05);
    }

    public async Task<(bool Success, string? Reason)> ReserveAsync(
        Guid orderId,
        (string Sku, int Quantity)[] items,
        CancellationToken cancellationToken = default)
    {
        // Simulate processing delay
        await Task.Delay(_random.Next(50, 200), cancellationToken);

        var shouldFail = _random.NextDouble() < _failureRate;

        if (shouldFail)
        {
            var skuIndex = _random.Next(items.Length);
            var reason = $"Insufficient inventory for SKU {items[skuIndex].Sku}";

            _logger.LogWarning(
                "Inventory reservation failed for Order {OrderId}: {Reason}",
                orderId, reason);

            return (false, reason);
        }

        _logger.LogInformation(
            "Inventory reserved for Order {OrderId}: {ItemCount} items",
            orderId, items.Length);

        return (true, null);
    }

    public async Task ReleaseAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        // Simulate processing delay
        await Task.Delay(_random.Next(50, 100), cancellationToken);

        _logger.LogInformation(
            "Inventory released for Order {OrderId}",
            orderId);
    }
}
