using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Api.Services;
using OrderProcessing.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                  "http://localhost:5173",
                  "https://localhost:5173",
                  "http://localhost:3000",
                  "https://localhost:3000"
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Host=localhost;Port=5432;Database=orderprocessing;Username=postgres;Password=postgres"
    ));

// MassTransit
builder.Services.AddMassTransit(x =>
{
    var transportType = builder.Configuration.GetValue<string>("MassTransit:Transport") ?? "RabbitMQ";

    if (transportType == "AzureServiceBus")
    {
        x.UsingAzureServiceBus((context, cfg) =>
        {
            cfg.Host(builder.Configuration.GetConnectionString("AzureServiceBus"));
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
            cfg.ConfigureEndpoints(context);
        });
    }
});

builder.Services.AddHostedService<OutboxPublisher>();

builder.Services.AddControllers();

var app = builder.Build();

// Enable CORS
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Avoid HTTPS redirection in development/container where HTTPS isn't bound
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

// Ensure database is migrated at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
