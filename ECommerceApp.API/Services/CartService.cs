using ECommerceApp.API.Data;
using ECommerceApp.API.Dtos;
using ECommerceApp.API.Models;
using ECommerceApp.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.API.Services;

public class CartService : ICartService
{
    private readonly AppDbContext _context;

    public CartService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<object> AddToCartAsync(AddToCartRequest request)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == request.UserId && ci.ProductId == request.ProductId);

        if (cartItem == null)
        {
            cartItem = new Models.CartItem
            {
                UserId = request.UserId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };
            _context.CartItems.Add(cartItem);
        }
        else
        {
            cartItem.Quantity += request.Quantity;
        }

        await _context.SaveChangesAsync();
        return cartItem;
    }

    public async Task<object> GetCartAsync(string userId)
    {
        var cartItems = await _context.CartItems
        .Include(c => c.Product)
        .Where(c => c.UserId == userId)
        .ToListAsync();

        var cart = cartItems.Select(c => new
        {
            c.Id,
            c.ProductId,
            Product = new
            {
                c.Product!.Name,
                c.Product.ImageUrl,
                OriginalPrice = c.Product.Price,
                DiscountedPrice = GetDiscountedPrice(c.Product)
            },
            c.Quantity
        });

        return cart;
    }

    private decimal? GetDiscountedPrice(Product product)
    {
        if (product.DiscountPercent.HasValue &&
            product.DiscountStartDate <= DateTime.Today &&
            DateTime.Today <= product.DiscountEndDate)
        {
            return product.Price * (1 - product.DiscountPercent.Value / 100);
        }

        return null;
    }
    public async Task<bool> RemoveFromCartAsync(string userId, int productId)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

        if (cartItem == null)
            return false;

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<object?> UpdateCartItemAsync(string userId, int productId, UpdateCartItemRequest request)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

        if (cartItem == null)
            return null;

        if (request.Quantity <= 0)
        {
            _context.CartItems.Remove(cartItem);
        }
        else
        {
            cartItem.Quantity = request.Quantity;
        }

        await _context.SaveChangesAsync();
        return cartItem;
    }
}
