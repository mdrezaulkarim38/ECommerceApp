namespace ECommerceApp.API.Models;

public class WishlistItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public AppUser? User { get; set; }
    public Product? Product { get; set; }
}
