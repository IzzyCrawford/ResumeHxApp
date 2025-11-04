using MassTransit;
using OrderProcessing.Contracts.V1;
using OrderProcessing.Infrastructure.Providers;

namespace OrderProcessing.Worker.Consumers;

public class EmailSendConsumer : IConsumer<EmailSendRequestV1>
{
    private readonly IMockEmailProvider _emailProvider;
    private readonly ILogger<EmailSendConsumer> _logger;

    public EmailSendConsumer(
        IMockEmailProvider emailProvider,
        ILogger<EmailSendConsumer> logger)
    {
        _emailProvider = emailProvider;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EmailSendRequestV1> context)
    {
        var message = context.Message;
        _logger.LogInformation(
            "Sending email for Order {OrderId}, Template: {Template}",
            message.OrderId, message.Template);

        try
        {
            var (sent, reason) = await _emailProvider.SendAsync(
                message.OrderId,
                message.To,
                message.Template,
                message.Model,
                context.CancellationToken);

            // Reply with result
            await context.RespondAsync(new EmailSendResultV1(
                OrderId: message.OrderId,
                Sent: sent,
                Reason: reason,
                CorrelationId: message.CorrelationId
            ));

            _logger.LogInformation(
                "Email send for Order {OrderId} completed: Sent={Sent}",
                message.OrderId, sent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email for Order {OrderId}", message.OrderId);

            await context.RespondAsync(new EmailSendResultV1(
                OrderId: message.OrderId,
                Sent: false,
                Reason: $"Internal error: {ex.Message}",
                CorrelationId: message.CorrelationId
            ));
        }
    }
}
