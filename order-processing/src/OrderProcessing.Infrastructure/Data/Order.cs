using System.ComponentModel.DataAnnotations;

namespace OrderProcessing.Infrastructure.Data;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(200)]
    public string CustomerId { get; set; } = string.Empty;
    
    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Created;
    
    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "USD";
    
    [Required]
    [MaxLength(200)]
    public string IdempotencyKey { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string CorrelationId { get; set; } = string.Empty;
    
    // Shipping Address
    [Required]
    [MaxLength(200)]
    public string ShippingName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(300)]
    public string ShippingLine1 { get; set; } = string.Empty;
    
    [MaxLength(300)]
    public string? ShippingLine2 { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string ShippingCity { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string ShippingState { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string ShippingPostalCode { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string ShippingCountry { get; set; } = string.Empty;
    
    // Amounts
    public decimal ShippingCost { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public decimal TaxRate { get; set; } = 0.0985m; // 9.85%
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<InventoryReservation> InventoryReservations { get; set; } = new List<InventoryReservation>();
    public ICollection<OrderEvent> Events { get; set; } = new List<OrderEvent>();
}
