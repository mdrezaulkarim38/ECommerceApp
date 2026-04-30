using ECommerceApp.API.Data;
using ECommerceApp.API.Dtos;
using ECommerceApp.API.Models;
using ECommerceApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.API.Controllers;

[ApiController]
[Authorize]
[Route("api/wishlist")]
public class WishlistController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly CurrentUserService _currentUser;

    public WishlistController(AppDbContext db, CurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WishlistItemDto>>> GetWishlist(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var now = DateTimeOffset.UtcNow;
        var items = await _db.WishlistItems
            .AsNoTracking()
            .Include(w => w.Product)
                .ThenInclude(p => p!.Images)
            .Where(w => w.UserId == userId && w.Product!.IsActive)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(items.Select(w =>
        {
            var product = w.Product!;
            var image = product.Images
                .OrderByDescending(i => i.IsPrimary)
                .ThenBy(i => i.SortOrder)
                .FirstOrDefault();

            return new WishlistItemDto(
                product.Id,
                product.Name,
                product.Slug,
                image?.Url,
                product.Price,
                product.GetEffectivePrice(now),
                w.CreatedAt);
        }).ToList());
    }

    [HttpPost("{productId:guid}")]
    public async Task<IActionResult> AddToWishlist(Guid productId, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var productExists = await _db.Products.AnyAsync(p => p.Id == productId && p.IsActive, cancellationToken);
        if (!productExists)
        {
            return NotFound(new ApiMessage("Product was not found."));
        }

        var alreadyExists = await _db.WishlistItems
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId, cancellationToken);

        if (!alreadyExists)
        {
            _db.WishlistItems.Add(new WishlistItem
            {
                UserId = userId,
                ProductId = productId
            });

            await _db.SaveChangesAsync(cancellationToken);
        }

        return Ok(new ApiMessage("Product added to wishlist."));
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> RemoveFromWishlist(Guid productId, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var item = await _db.WishlistItems
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId, cancellationToken);

        if (item is null)
        {
            return NotFound(new ApiMessage("Wishlist item was not found."));
        }

        _db.WishlistItems.Remove(item);
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new ApiMessage("Product removed from wishlist."));
    }
}
