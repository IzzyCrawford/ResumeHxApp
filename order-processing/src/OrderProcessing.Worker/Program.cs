using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Infrastructure.Data;
using OrderProcessing.Infrastructure.Providers;
using OrderProcessing.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Host=localhost;Port=5432;Database=orderprocessing;Username=postgres;Password=postgres"
    ));

// Mock Providers
builder.Services.AddSingleton<IMockPayProvider, MockPayProvider>();
builder.Services.AddSingleton<IMockInventoryProvider, MockInventoryProvider>();
builder.Services.AddSingleton<IMockEmailProvider, MockEmailProvider>();

// MassTransit
builder.Services.AddMassTransit(x =>
{
    // Add consumers
    x.AddConsumer<OrderCreatedConsumer>();
    x.AddConsumer<InventoryReserveConsumer>();
    x.AddConsumer<PaymentAuthorizeConsumer>();
    x.AddConsumer<EmailSendConsumer>();

    var transportType = builder.Configuration.GetValue<string>("MassTransit:Transport") ?? "RabbitMQ";

    if (transportType == "AzureServiceBus")
    {
        x.UsingAzureServiceBus((context, cfg) =>
        {
            cfg.Host(builder.Configuration.GetConnectionString("AzureServiceBus"));

            // Configure retry policy: 3 attempts with intervals
            cfg.UseMessageRetry(r =>
            {
                r.Intervals(
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(15),
                    TimeSpan.FromSeconds(60)
                );
            });

            cfg.ConfigureEndpoints(context);
        });
    }
    else
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(builder.Configuration.GetValue<string>("MassTransit:RabbitMQ:Host") ?? "localhost", "/", h =>
            {
                h.Username(builder.Configuration.GetValue<string>("MassTransit:RabbitMQ:Username") ?? "guest");
                h.Password(builder.Configuration.GetValue<string>("MassTransit:RabbitMQ:Password") ?? "guest");
            });

            // Configure retry policy: 3 attempts with intervals
            cfg.UseMessageRetry(r =>
            {
                r.Intervals(
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(15),
                    TimeSpan.FromSeconds(60)
                );
            });

            cfg.ConfigureEndpoints(context);
        });
    }
});

var host = builder.Build();

// Ensure database is migrated at startup
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
host.Run();
