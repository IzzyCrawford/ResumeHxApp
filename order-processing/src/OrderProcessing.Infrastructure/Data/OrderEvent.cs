using System.ComponentModel.DataAnnotations;

namespace OrderProcessing.Infrastructure.Data;

public class OrderEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid OrderId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string EventType { get; set; } = string.Empty;
    
    [Required]
    public string Payload { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public Order Order { get; set; } = null!;
}
