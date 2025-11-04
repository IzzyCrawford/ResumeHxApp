namespace OrderProcessing.Infrastructure.Providers;

public interface IMockEmailProvider
{
    Task<(bool Sent, string? Reason)> SendAsync(
        Guid orderId,
        string to,
        string template,
        object model,
        CancellationToken cancellationToken = default);
}
