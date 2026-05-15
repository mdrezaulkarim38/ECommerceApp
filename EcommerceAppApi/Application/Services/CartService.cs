using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using EcommerceAppApi.Domain.Entities;
using EcommerceAppApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAppApi.Application.Services;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;
    public CartService(ApplicationDbContext context) => _context = context;

    public async Task<CartDto> GetCartAsync(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null) return new CartDto { Items = new() };

        return MapToDto(cart);
    }

    public async Task<CartDto> AddToCartAsync(int userId, AddToCartRequest request)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId, CreatedAt = DateTime.UtcNow };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var existingItem = cart.Items.FirstOrDefault(ci => ci.ProductId == request.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                AddedAt = DateTime.UtcNow
            });
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _context.Entry(cart).Collection(c => c.Items).Query().Include(ci => ci.Product).LoadAsync();
        return MapToDto(cart);
    }

    public async Task<CartDto> UpdateCartItemAsync(int userId, int productId, UpdateCartItemRequest request)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null) throw new KeyNotFoundException("Cart not found");

        var item = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);
        if (item == null) throw new KeyNotFoundException("Item not found in cart");

        if (request.Quantity <= 0)
        {
            _context.CartItems.Remove(item);
        }
        else
        {
            item.Quantity = request.Quantity;
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _context.Entry(cart).Collection(c => c.Items).Query().Include(ci => ci.Product).LoadAsync();
        return MapToDto(cart);
    }

    public async Task<CartDto> RemoveFromCartAsync(int userId, int productId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null) throw new KeyNotFoundException("Cart not found");

        var item = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);
        if (item != null)
        {
            _context.CartItems.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        await _context.Entry(cart).Collection(c => c.Items).Query().Include(ci => ci.Product).LoadAsync();
        return MapToDto(cart);
    }

    public async Task ClearCartAsync(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart != null)
        {
            _context.CartItems.RemoveRange(cart.Items);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetCartCountAsync(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        return cart?.Items.Sum(i => i.Quantity) ?? 0;
    }

    private static CartDto MapToDto(Cart cart)
    {
        var items = cart.Items.Select(ci => new CartItemDto
        {
            ProductId = ci.ProductId,
            ProductName = ci.Product.Name,
            ProductImageUrl = ci.Product.MainImageUrl ?? "",
            UnitPrice = ci.Product.Price,
            Quantity = ci.Quantity,
            TotalPrice = ci.Product.Price * ci.Quantity,
            MaxStock = ci.Product.StockQuantity
        }).ToList();

        return new CartDto
        {
            Id = cart.Id,
            Items = items,
            Subtotal = items.Sum(i => i.TotalPrice),
            TotalItems = items.Sum(i => i.Quantity)
        };
    }
}
