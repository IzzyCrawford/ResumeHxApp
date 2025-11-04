using System.ComponentModel.DataAnnotations;

namespace OrderProcessing.Infrastructure.Data;

public enum ReservationStatus
{
    Reserved,
    Released,
    Failed
}

public class InventoryReservation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid OrderId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Sku { get; set; } = string.Empty;
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Required]
    public ReservationStatus Status { get; set; }
    
    public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ReleasedAt { get; set; }
    
    [MaxLength(1000)]
    public string? FailureReason { get; set; }
    
    // Navigation
    public Order Order { get; set; } = null!;
}
