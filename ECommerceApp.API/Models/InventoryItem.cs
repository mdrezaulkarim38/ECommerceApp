namespace ECommerceApp.API.Models;

public class InventoryItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public int QuantityAvailable { get; set; }
    public int QuantityReserved { get; set; }
    public int LowStockThreshold { get; set; } = 5;
    public DateTimeOffset? LastRestockedAt { get; set; }

    public Product? Product { get; set; }
}
