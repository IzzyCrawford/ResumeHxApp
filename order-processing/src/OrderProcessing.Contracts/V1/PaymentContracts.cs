namespace OrderProcessing.Contracts.V1;

public record PaymentAuthorizeRequestV1(
    Guid OrderId,
    decimal Amount,
    string Currency,
    string CorrelationId,
    int SchemaVersion = 1,
    DateTime OccurredAt = default
)
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}

public record PaymentAuthorizeResultV1(
    Guid OrderId,
    bool Authorized,
    string? IntentId,
    string? Reason,
    string CorrelationId,
    int SchemaVersion = 1,
    DateTime OccurredAt = default
)
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}
