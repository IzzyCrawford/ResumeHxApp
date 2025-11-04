namespace OrderProcessing.Contracts.V1;

public record OrderUpdatedV1(
    Guid OrderId,
    string Status,
    string? Reason,
    string CorrelationId,
    int SchemaVersion = 1,
    DateTime OccurredAt = default
)
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}
