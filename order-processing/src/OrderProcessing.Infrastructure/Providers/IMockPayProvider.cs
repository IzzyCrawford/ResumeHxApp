namespace OrderProcessing.Infrastructure.Providers;

public interface IMockPayProvider
{
    Task<(bool Authorized, string? IntentId, string? Reason)> AuthorizeAsync(
        Guid orderId, 
        decimal amount, 
        string currency,
        CancellationToken cancellationToken = default);
}
