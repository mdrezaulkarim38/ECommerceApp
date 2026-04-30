using Microsoft.AspNetCore.Identity;

namespace ECommerceApp.API.Models;

public class AppUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastLoginAt { get; set; }

    public Cart? Cart { get; set; }
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<WishlistItem> WishlistItems { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}
