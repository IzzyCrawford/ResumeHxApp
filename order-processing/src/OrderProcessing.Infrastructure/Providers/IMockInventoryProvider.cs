namespace OrderProcessing.Infrastructure.Providers;

public interface IMockInventoryProvider
{
    Task<(bool Success, string? Reason)> ReserveAsync(
        Guid orderId,
        (string Sku, int Quantity)[] items,
        CancellationToken cancellationToken = default);

    Task ReleaseAsync(Guid orderId, CancellationToken cancellationToken = default);
}
