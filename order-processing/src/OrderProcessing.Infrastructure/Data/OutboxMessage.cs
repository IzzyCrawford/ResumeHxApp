using System.ComponentModel.DataAnnotations;

namespace OrderProcessing.Infrastructure.Data;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(200)]
    public string Type { get; set; } = string.Empty;
    
    [Required]
    public string Payload { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string CorrelationId { get; set; } = string.Empty;
    
    public int Attempts { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? PublishedAt { get; set; }
    
    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }
}
