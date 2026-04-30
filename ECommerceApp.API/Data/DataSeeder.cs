using ECommerceApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.API.Data;

public class DataSeeder
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IConfiguration _configuration;

    public DataSeeder(
        AppDbContext db,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IConfiguration configuration)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminAsync();
        await SeedCatalogAsync();
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleName in new[] { "Admin", "Customer" })
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }
    }

    private async Task SeedAdminAsync()
    {
        var email = _configuration["Seed:AdminEmail"] ?? "admin@ecommerce.local";
        var password = _configuration["Seed:AdminPassword"] ?? "Admin@12345";
        var admin = await _userManager.FindByEmailAsync(email);

        if (admin is null)
        {
            admin = new AppUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = "Store Administrator",
                CreatedAt = DateTimeOffset.UtcNow
            };

            var result = await _userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
            {
                var message = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Could not seed admin user: {message}");
            }
        }

        if (!await _userManager.IsInRoleAsync(admin, "Admin"))
        {
            await _userManager.AddToRoleAsync(admin, "Admin");
        }
    }

    private async Task SeedCatalogAsync()
    {
        if (await _db.Categories.AnyAsync())
        {
            return;
        }

        var electronics = new Category
        {
            Name = "Electronics",
            Slug = "electronics",
            Description = "Phones, smart devices, and accessories."
        };
        var fashion = new Category
        {
            Name = "Fashion",
            Slug = "fashion",
            Description = "Wearables, bags, and daily style essentials."
        };
        var home = new Category
        {
            Name = "Home & Living",
            Slug = "home-living",
            Description = "Useful items for a smarter home."
        };

        _db.Categories.AddRange(electronics, fashion, home);

        _db.Products.AddRange(
            CreateProduct("Smart Fitness Band", "smart-fitness-band", electronics, "SFB-1001", 3490, 25, true),
            CreateProduct("Wireless Noise Cancelling Headphones", "wireless-noise-cancelling-headphones", electronics, "WNH-2001", 8990, 14, true),
            CreateProduct("Minimal Travel Backpack", "minimal-travel-backpack", fashion, "MTB-3001", 2490, 30, false),
            CreateProduct("Cotton Casual Shirt", "cotton-casual-shirt", fashion, "CCS-4001", 1290, 42, false),
            CreateProduct("Smart LED Desk Lamp", "smart-led-desk-lamp", home, "SLD-5001", 1890, 12, true),
            CreateProduct("Ceramic Dinner Set", "ceramic-dinner-set", home, "CDS-6001", 3190, 8, false));

        await _db.SaveChangesAsync();
    }

    private static Product CreateProduct(
        string name,
        string slug,
        Category category,
        string sku,
        decimal price,
        int stock,
        bool featured)
    {
        return new Product
        {
            Name = name,
            Slug = slug,
            Description = $"{name} prepared as starter catalog data for the ecommerce platform.",
            Category = category,
            Sku = sku,
            Price = price,
            IsFeatured = featured,
            Inventory = new InventoryItem
            {
                QuantityAvailable = stock,
                LowStockThreshold = 5,
                LastRestockedAt = DateTimeOffset.UtcNow
            }
        };
    }
}
