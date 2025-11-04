using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OrderProcessing.Infrastructure.Providers;

public class MockEmailProvider : IMockEmailProvider
{
    private readonly ILogger<MockEmailProvider> _logger;
    private readonly double _failureRate;
    private readonly Random _random = new();

    public MockEmailProvider(IConfiguration configuration, ILogger<MockEmailProvider> logger)
    {
        _logger = logger;
        _failureRate = configuration.GetValue<double>("MockProviders:Email:FailureRate", 0.02);
    }

    public async Task<(bool Sent, string? Reason)> SendAsync(
        Guid orderId,
        string to,
        string template,
        object model,
        CancellationToken cancellationToken = default)
    {
        // Simulate processing delay
        await Task.Delay(_random.Next(100, 300), cancellationToken);

        var shouldFail = _random.NextDouble() < _failureRate;

        if (shouldFail)
        {
            var reasons = new[]
            {
                "SMTP server timeout",
                "Invalid recipient email address",
                "Email service unavailable"
            };
            var reason = reasons[_random.Next(reasons.Length)];

            _logger.LogWarning(
                "Email send failed for Order {OrderId}: {Reason}",
                orderId, reason);

            return (false, reason);
        }

        _logger.LogInformation(
            "Email sent for Order {OrderId}: To={EmailAddress}, Template={Template}",
            orderId, to, template);

        return (true, null);
    }
}
