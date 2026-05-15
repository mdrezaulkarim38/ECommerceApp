namespace EcommerceAppApi.Application.DTOs;

public class CheckoutQuoteRequest
{
    public List<CartItemForQuoteDto> CartItems { get; set; } = new();
    public int? AddressId { get; set; }
    public AddressDto? Address { get; set; }
    public string? CouponCode { get; set; }
}

public class CartItemForQuoteDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CheckoutQuoteResponse
{
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? AppliedCouponCode { get; set; }
}

public class PlaceOrderRequest
{
    public int? AddressId { get; set; }
    public AddressDto? ShippingAddress { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? CouponCode { get; set; }
}

public class OrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ItemCount { get; set; }
}

public class OrderDetailDto : OrderDto
{
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? TrackingNumber { get; set; }
    public string? TrackingUrl { get; set; }
    public AddressDto ShippingAddress { get; set; } = null!;
    public List<OrderItemDto> Items { get; set; } = new();
    public List<OrderStatusHistoryDto> StatusHistory { get; set; } = new();
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}

public class OrderStatusHistoryDto
{
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime ChangedAt { get; set; }
}

public class AddressDto
{
    public int? Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}