using System.ComponentModel.DataAnnotations;

namespace OrderProcessing.Infrastructure.Data;

public enum PaymentStatus
{
    Authorized,
    Failed
}

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid OrderId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Provider { get; set; } = "MockPay";
    
    [MaxLength(200)]
    public string? IntentId { get; set; }
    
    [Required]
    public PaymentStatus Status { get; set; }
    
    [Required]
    public decimal Amount { get; set; }
    
    public DateTime? AuthorizedAt { get; set; }
    
    [MaxLength(1000)]
    public string? FailureReason { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public Order Order { get; set; } = null!;
}
