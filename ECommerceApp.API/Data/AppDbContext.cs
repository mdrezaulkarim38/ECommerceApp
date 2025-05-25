using ECommerceApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.API.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
}