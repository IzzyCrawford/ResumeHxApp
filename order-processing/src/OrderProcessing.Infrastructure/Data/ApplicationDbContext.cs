using Microsoft.EntityFrameworkCore;

namespace OrderProcessing.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }
    
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<InventoryReservation> InventoryReservations => Set<InventoryReservation>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<OrderEvent> OrderEvents => Set<OrderEvent>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Order entity configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            
            entity.Property(o => o.Status)
                .HasConversion<string>();
            
            entity.HasIndex(o => new { o.IdempotencyKey, o.CustomerId })
                .IsUnique()
                .HasFilter("[IdempotencyKey] IS NOT NULL");
            
            entity.HasIndex(o => o.CorrelationId);
            
            entity.HasIndex(o => new { o.Status, o.CreatedAt });
            
            entity.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(o => o.Payments)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(o => o.InventoryReservations)
                .WithOne(r => r.Order)
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(o => o.Events)
                .WithOne(e => e.Order)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // OrderItem entity configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(i => i.Id);
            
            entity.HasIndex(i => i.OrderId);
        });
        
        // Payment entity configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.Status)
                .HasConversion<string>();
            
            entity.HasIndex(p => p.OrderId);
            
            entity.HasIndex(p => p.IntentId)
                .IsUnique()
                .HasFilter("[IntentId] IS NOT NULL");
        });
        
        // InventoryReservation entity configuration
        modelBuilder.Entity<InventoryReservation>(entity =>
        {
            entity.HasKey(r => r.Id);
            
            entity.Property(r => r.Status)
                .HasConversion<string>();
            
            entity.HasIndex(r => r.OrderId);
            
            entity.HasIndex(r => new { r.Sku, r.Status });
        });
        
        // OutboxMessage entity configuration
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(m => m.Id);
            
            entity.HasIndex(m => m.PublishedAt)
                .HasFilter("[PublishedAt] IS NULL");
            
            entity.HasIndex(m => m.CreatedAt);
        });
        
        // OrderEvent entity configuration
        modelBuilder.Entity<OrderEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => new { e.OrderId, e.CreatedAt });
        });
    }
}
