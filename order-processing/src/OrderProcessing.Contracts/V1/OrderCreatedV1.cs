namespace OrderProcessing.Contracts.V1;

public record OrderItemContract(
    string Sku,
    string Name,
    int Quantity,
    decimal UnitPrice,
    bool IsTaxable = true
);

public record OrderCreatedV1(
    Guid OrderId,
    string CustomerId,
    string Currency,
    OrderItemContract[] Items,
    decimal Subtotal,
    decimal ShippingCost,
    decimal Tax,
    decimal Total,
    string CorrelationId,
    int SchemaVersion = 1,
    DateTime OccurredAt = default
)
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}
