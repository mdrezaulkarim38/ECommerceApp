namespace ECommerceApp.API.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal? DiscountPercent { get; set; }
    public DateTimeOffset? DiscountStartAt { get; set; }
    public DateTimeOffset? DiscountEndAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }

    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    public InventoryItem? Inventory { get; set; }
    public ICollection<ProductImage> Images { get; set; } = [];
    public ICollection<CartItem> CartItems { get; set; } = [];
    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public ICollection<WishlistItem> WishlistItems { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];

    public decimal GetEffectivePrice(DateTimeOffset now)
    {
        if (DiscountPercent is > 0 &&
            (!DiscountStartAt.HasValue || DiscountStartAt.Value <= now) &&
            (!DiscountEndAt.HasValue || now <= DiscountEndAt.Value))
        {
            return Math.Round(Price * (1 - DiscountPercent.Value / 100), 2);
        }

        return Price;
    }
}
