namespace ECommerceApp.API.Models;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5,
    Refunded = 6
}

public enum PaymentStatus
{
    Pending = 0,
    Paid = 1,
    Failed = 2,
    Refunded = 3
}

public enum PaymentMethod
{
    CashOnDelivery = 0,
    Card = 1,
    MobileBanking = 2
}
