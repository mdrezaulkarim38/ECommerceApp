namespace ECommerceApp.API.Dtos;

public record AddCartItemRequest(Guid ProductId, int Quantity);

public record UpdateCartItemRequest(int Quantity);

public record CartItemDto(
    Guid ProductId,
    string ProductName,
    string Slug,
    string? ImageUrl,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);

public record CartDto(
    Guid CartId,
    IReadOnlyList<CartItemDto> Items,
    int TotalQuantity,
    decimal Subtotal);
