using ECommerceApp.API.Models;

namespace ECommerceApp.API.Dtos;

public record CheckoutRequest(
    string ShippingName,
    string ShippingPhone,
    string ShippingAddressLine1,
    string ShippingCity,
    string ShippingCountry,
    PaymentMethod PaymentMethod,
    string? Notes);

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    string Sku,
    decimal UnitPrice,
    decimal DiscountAmount,
    int Quantity,
    decimal LineTotal);

public record OrderDto(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    PaymentStatus PaymentStatus,
    PaymentMethod PaymentMethod,
    decimal Subtotal,
    decimal DiscountTotal,
    decimal ShippingTotal,
    decimal TaxTotal,
    decimal Total,
    string ShippingName,
    string ShippingPhone,
    string ShippingAddressLine1,
    string ShippingCity,
    string ShippingCountry,
    string? Notes,
    DateTimeOffset CreatedAt,
    IReadOnlyList<OrderItemDto> Items);

public record UpdateOrderStatusRequest(OrderStatus Status, PaymentStatus? PaymentStatus);
