namespace OrderProcessing.Contracts.V1;

public record EmailSendRequestV1(
    Guid OrderId,
    string To,
    string Template,
    object Model,
    string CorrelationId,
    int SchemaVersion = 1,
    DateTime OccurredAt = default
)
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}

public record EmailSendResultV1(
    Guid OrderId,
    bool Sent,
    string? Reason,
    string CorrelationId,
    int SchemaVersion = 1,
    DateTime OccurredAt = default
)
{
    public DateTime OccurredAt { get; init; } = OccurredAt == default ? DateTime.UtcNow : OccurredAt;
}
