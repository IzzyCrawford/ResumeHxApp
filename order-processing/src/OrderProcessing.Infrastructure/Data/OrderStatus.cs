namespace OrderProcessing.Infrastructure.Data;

public enum OrderStatus
{
    Created,
    Accepted,
    InventoryReserved,
    PaymentAuthorized,
    Confirmed,
    FailedInventory,
    FailedPayment,
    EmailFailed,
    Cancelled,
    Failed
}
