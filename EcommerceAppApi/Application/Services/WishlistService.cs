using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using EcommerceAppApi.Domain.Entities;
using EcommerceAppApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAppApi.Application.Services;

public class WishlistService : IWishlistService
{
    private readonly ApplicationDbContext _context;
    public WishlistService(ApplicationDbContext context) => _context = context;

    public async Task<List<ProductDto>> GetWishlistAsync(int userId)
    {
        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
                .ThenInclude(wi => wi.Product)
                    .ThenInclude(p => p.Category)
            .Include(w => w.Items)
                .ThenInclude(wi => wi.Product)
                    .ThenInclude(p => p.Brand)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wishlist == null) return new();

        return wishlist.Items
            .Where(wi => wi.Product.IsActive)
            .Select(wi => new ProductDto
            {
                Id = wi.Product.Id,
                Name = wi.Product.Name,
                Slug = wi.Product.Slug,
                Description = wi.Product.Description,
                Price = wi.Product.Price,
                CompareAtPrice = wi.Product.CompareAtPrice,
                StockQuantity = wi.Product.StockQuantity,
                CategoryId = wi.Product.CategoryId,
                CategoryName = wi.Product.Category?.Name,
                BrandId = wi.Product.BrandId,
                BrandName = wi.Product.Brand?.Name,
                MainImageUrl = wi.Product.MainImageUrl,
                AverageRating = wi.Product.AverageRating,
                TotalReviews = wi.Product.TotalReviews,
                IsActive = wi.Product.IsActive
            })
            .ToList();
    }

    public async Task<bool> ToggleWishlistAsync(int userId, int productId)
    {
        var wishlist = await _context.Wishlists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wishlist == null)
        {
            wishlist = new Wishlist { UserId = userId, CreatedAt = DateTime.UtcNow };
            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();
        }

        var existingItem = wishlist.Items.FirstOrDefault(wi => wi.ProductId == productId);
        if (existingItem != null)
        {
            _context.WishlistItems.Remove(existingItem);
            await _context.SaveChangesAsync();
            return false;
        }

        wishlist.Items.Add(new WishlistItem
        {
            WishlistId = wishlist.Id,
            ProductId = productId,
            AddedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsInWishlistAsync(int userId, int productId)
    {
        return await _context.WishlistItems
            .AnyAsync(wi => wi.Wishlist.UserId == userId && wi.ProductId == productId);
    }
}
