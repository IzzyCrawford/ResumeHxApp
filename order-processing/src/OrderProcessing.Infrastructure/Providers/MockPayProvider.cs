using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OrderProcessing.Infrastructure.Providers;

public class MockPayProvider : IMockPayProvider
{
    private readonly ILogger<MockPayProvider> _logger;
    private readonly double _failureRate;
    private readonly Random _random = new();

    public MockPayProvider(IConfiguration configuration, ILogger<MockPayProvider> logger)
    {
        _logger = logger;
        _failureRate = configuration.GetValue<double>("MockProviders:Pay:FailureRate", 0.10);
    }

    public async Task<(bool Authorized, string? IntentId, string? Reason)> AuthorizeAsync(
        Guid orderId,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default)
    {
        // Simulate processing delay
        await Task.Delay(_random.Next(100, 500), cancellationToken);

        var shouldFail = _random.NextDouble() < _failureRate;

        if (shouldFail)
        {
            var reasons = new[]
            {
                "Insufficient funds",
                "Card declined",
                "Invalid card details",
                "Payment processor timeout"
            };
            var reason = reasons[_random.Next(reasons.Length)];

            _logger.LogWarning(
                "Payment authorization failed for Order {OrderId}: {Reason}",
                orderId, reason);

            return (false, null, reason);
        }

        var intentId = $"pi_{Guid.NewGuid():N}";

        _logger.LogInformation(
            "Payment authorized for Order {OrderId}: Amount={Amount} {Currency}, IntentId={IntentId}",
            orderId, amount, currency, intentId);

        return (true, intentId, null);
    }
}
