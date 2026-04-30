namespace ECommerceApp.API.Models;

public class Cart
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public AppUser? User { get; set; }
    public ICollection<CartItem> Items { get; set; } = [];
}
