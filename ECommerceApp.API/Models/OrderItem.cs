namespace ECommerceApp.API.Models;

public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }

    public Order? Order { get; set; }
    public Product? Product { get; set; }
}
