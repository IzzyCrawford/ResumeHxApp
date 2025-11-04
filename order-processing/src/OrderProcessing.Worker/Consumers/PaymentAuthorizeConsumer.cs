using MassTransit;
using OrderProcessing.Contracts.V1;
using OrderProcessing.Infrastructure.Data;
using OrderProcessing.Infrastructure.Providers;

namespace OrderProcessing.Worker.Consumers;

public class PaymentAuthorizeConsumer : IConsumer<PaymentAuthorizeRequestV1>
{
    private readonly ApplicationDbContext _context;
    private readonly IMockPayProvider _payProvider;
    private readonly ILogger<PaymentAuthorizeConsumer> _logger;

    public PaymentAuthorizeConsumer(
        ApplicationDbContext context,
        IMockPayProvider payProvider,
        ILogger<PaymentAuthorizeConsumer> logger)
    {
        _context = context;
        _payProvider = payProvider;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentAuthorizeRequestV1> context)
    {
        var message = context.Message;
        _logger.LogInformation(
            "Authorizing payment for Order {OrderId}, Amount: {Amount} {Currency}",
            message.OrderId, message.Amount, message.Currency);

        try
        {
            var (authorized, intentId, reason) = await _payProvider.AuthorizeAsync(
                message.OrderId,
                message.Amount,
                message.Currency,
                context.CancellationToken);

            // Save payment record
            var payment = new Payment
            {
                OrderId = message.OrderId,
                Provider = "MockPay",
                IntentId = intentId,
                Status = authorized ? PaymentStatus.Authorized : PaymentStatus.Failed,
                Amount = message.Amount,
                AuthorizedAt = authorized ? DateTime.UtcNow : null,
                FailureReason = reason
            };
            _context.Payments.Add(payment);

            await _context.SaveChangesAsync(context.CancellationToken);

            // Reply with result
            await context.RespondAsync(new PaymentAuthorizeResultV1(
                OrderId: message.OrderId,
                Authorized: authorized,
                IntentId: intentId,
                Reason: reason,
                CorrelationId: message.CorrelationId
            ));

            _logger.LogInformation(
                "Payment authorization for Order {OrderId} completed: Authorized={Authorized}, IntentId={IntentId}",
                message.OrderId, authorized, intentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authorizing payment for Order {OrderId}", message.OrderId);

            await context.RespondAsync(new PaymentAuthorizeResultV1(
                OrderId: message.OrderId,
                Authorized: false,
                IntentId: null,
                Reason: $"Internal error: {ex.Message}",
                CorrelationId: message.CorrelationId
            ));
        }
    }
}
