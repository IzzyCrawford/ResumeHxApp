using System.ComponentModel.DataAnnotations;

namespace OrderProcessing.Api.Models;

public record CreateOrderItemRequest(
    [Required][MaxLength(100)] string Sku,
    [Required][MaxLength(300)] string Name,
    [Required][Range(1, int.MaxValue)] int Quantity,
    [Required][Range(0.01, double.MaxValue)] decimal UnitPrice,
    bool IsTaxable = true
);

public record ShippingAddressRequest(
    [Required][MaxLength(200)] string Name,
    [Required][MaxLength(200)] string Line1,
    [MaxLength(200)] string? Line2,
    [Required][MaxLength(100)] string City,
    [Required][MaxLength(50)] string State,
    [Required][MaxLength(20)] string PostalCode,
    [Required][MaxLength(50)] string Country
);

public record CreateOrderRequest(
    [Required][MaxLength(200)] string CustomerId,
    [Required][MaxLength(3)] string Currency,
    [Required] ShippingAddressRequest ShippingAddress,
    [Required][Range(0, double.MaxValue)] decimal ShippingCost,
    [Required][MinLength(1)] CreateOrderItemRequest[] Items
);

public record OrderResponse(
    Guid OrderId,
    string Status,
    string CustomerId,
    string Currency,
    decimal Subtotal,
    decimal ShippingCost,
    decimal Tax,
    decimal Total,
    DateTime CreatedAt
);

public record OrderItemResponse(
    string Sku,
    string Name,
    int Quantity,
    decimal UnitPrice,
    bool IsTaxable
);

public record OrderDetailResponse(
    Guid OrderId,
    string Status,
    string CustomerId,
    string Currency,
    ShippingAddressRequest ShippingAddress,
    decimal Subtotal,
    decimal ShippingCost,
    decimal Tax,
    decimal Total,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    OrderItemResponse[] Items
);
