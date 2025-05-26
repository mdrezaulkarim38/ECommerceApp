namespace ECommerceApp.API.Dtos;
public class AddToCartRequest
{
    public string? UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}