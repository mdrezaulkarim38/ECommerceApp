using EcommerceAppApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAppApi.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) {}
    
    public DbSet<User> Users { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<BrandFollower> BrandFollowers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    public DbSet<Settings> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Category>().HasIndex(c => c.Slug).IsUnique();
        modelBuilder.Entity<Brand>().HasIndex(b => b.Slug).IsUnique();
        modelBuilder.Entity<Product>().HasIndex(p => p.Slug).IsUnique();
        modelBuilder.Entity<Product>().HasIndex(p => p.Sku).IsUnique();
        modelBuilder.Entity<Order>().HasIndex(o => o.OrderNumber).IsUnique();
        modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
        modelBuilder.Entity<Product>().Property(p => p.CompareAtPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(o => o.Subtotal).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(o => o.ShippingCost).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(o => o.TaxAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(o => o.DiscountAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasPrecision(18, 2);            
        modelBuilder.Entity<OrderItem>().Property(oi => oi.TotalPrice).HasPrecision(18, 2);
    }
}