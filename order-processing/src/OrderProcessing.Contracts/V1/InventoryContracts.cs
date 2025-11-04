namespace OrderProcessing.Contracts.V1;

public record InventoryItemContract(
    string Sku,
    int Quantity
);

public record InventoryReserveRequestV1(
    Guid OrderId,
    InventoryItemContract[] Items,
    string CorrelationId,
    int SchemaVersion = 1,
    DateTime OccurredAt = default
)
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}

public record InventoryReserveResultV1(
    Guid OrderId,
    bool Success,
    string? Reason,
    string CorrelationId,
    int SchemaVersion = 1,
    DateTime OccurredAt = default
)
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}
