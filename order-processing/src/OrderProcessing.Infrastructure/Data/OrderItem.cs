using System.ComponentModel.DataAnnotations;

namespace OrderProcessing.Infrastructure.Data;

public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid OrderId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Sku { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(300)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }
    
    public bool IsTaxable { get; set; } = true;
    
    // Navigation
    public Order Order { get; set; } = null!;
}
