namespace EcommerceAppApi.Application.DTOs;

public class CartDto
{
    public int Id { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public int TotalItems { get; set; }
}

public class CartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public int MaxStock { get; set; }
}

public class AddToCartRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateCartItemRequest
{
    public int Quantity { get; set; }
}

public class MoveToWishlistRequest
{
    public int ProductId { get; set; }
}