using ECommerceApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ECommerceApp.API.Data;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    private static readonly ValueConverter<DateTimeOffset, long> DateTimeOffsetToLongConverter = new(
        value => value.ToUnixTimeMilliseconds(),
        value => DateTimeOffset.FromUnixTimeMilliseconds(value));

    private static readonly ValueConverter<DateTimeOffset?, long?> NullableDateTimeOffsetToLongConverter = new(
        value => value.HasValue ? value.Value.ToUnixTimeMilliseconds() : null,
        value => value.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds(value.Value) : null);

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>(entity =>
        {
            entity.HasIndex(c => c.Slug).IsUnique();
            entity.Property(c => c.Name).HasMaxLength(120).IsRequired();
            entity.Property(c => c.Slug).HasMaxLength(140).IsRequired();
        });

        builder.Entity<Product>(entity =>
        {
            entity.HasIndex(p => p.Slug).IsUnique();
            entity.HasIndex(p => p.Sku).IsUnique();
            entity.Property(p => p.Name).HasMaxLength(160).IsRequired();
            entity.Property(p => p.Slug).HasMaxLength(180).IsRequired();
            entity.Property(p => p.Sku).HasMaxLength(64).IsRequired();
            entity.Property(p => p.Brand).HasMaxLength(80);
            entity.Property(p => p.Price).HasConversion<double>();
            entity.Property(p => p.CompareAtPrice).HasConversion<double?>();
            entity.Property(p => p.DiscountPercent).HasConversion<double?>();
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ProductImage>(entity =>
        {
            entity.Property(i => i.Url).HasMaxLength(900).IsRequired();
            entity.Property(i => i.AltText).HasMaxLength(160);
        });

        builder.Entity<InventoryItem>(entity =>
        {
            entity.HasIndex(i => i.ProductId).IsUnique();
            entity.HasOne(i => i.Product)
                .WithOne(p => p.Inventory)
                .HasForeignKey<InventoryItem>(i => i.ProductId);
        });

        builder.Entity<Cart>(entity =>
        {
            entity.HasIndex(c => c.UserId).IsUnique();
            entity.HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId);
        });

        builder.Entity<CartItem>(entity =>
        {
            entity.HasIndex(i => new { i.CartId, i.ProductId }).IsUnique();
        });

        builder.Entity<WishlistItem>(entity =>
        {
            entity.HasIndex(w => new { w.UserId, w.ProductId }).IsUnique();
        });

        builder.Entity<Order>(entity =>
        {
            entity.HasIndex(o => o.OrderNumber).IsUnique();
            entity.Property(o => o.Status).HasConversion<string>().HasMaxLength(32);
            entity.Property(o => o.PaymentStatus).HasConversion<string>().HasMaxLength(32);
            entity.Property(o => o.PaymentMethod).HasConversion<string>().HasMaxLength(32);
            entity.Property(o => o.Subtotal).HasConversion<double>();
            entity.Property(o => o.DiscountTotal).HasConversion<double>();
            entity.Property(o => o.ShippingTotal).HasConversion<double>();
            entity.Property(o => o.TaxTotal).HasConversion<double>();
            entity.Property(o => o.Total).HasConversion<double>();
        });

        builder.Entity<OrderItem>(entity =>
        {
            entity.Property(i => i.ProductName).HasMaxLength(160).IsRequired();
            entity.Property(i => i.Sku).HasMaxLength(64).IsRequired();
            entity.Property(i => i.UnitPrice).HasConversion<double>();
            entity.Property(i => i.DiscountAmount).HasConversion<double>();
            entity.Property(i => i.LineTotal).HasConversion<double>();
        });

        builder.Entity<Review>(entity =>
        {
            entity.HasIndex(r => new { r.UserId, r.ProductId }).IsUnique();
            entity.Property(r => r.Title).HasMaxLength(120);
            entity.Property(r => r.Comment).HasMaxLength(1200);
        });

        ConfigureSqliteDateTimeOffsets(builder);
    }

    private static void ConfigureSqliteDateTimeOffsets(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTimeOffset))
                {
                    property.SetValueConverter(DateTimeOffsetToLongConverter);
                    property.SetColumnType("INTEGER");
                }
                else if (property.ClrType == typeof(DateTimeOffset?))
                {
                    property.SetValueConverter(NullableDateTimeOffsetToLongConverter);
                    property.SetColumnType("INTEGER");
                }
            }
        }
    }
}
