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
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly CurrentUserService _currentUser;

    public CartController(AppDbContext db, CurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart(CancellationToken cancellationToken)
    {
        var cart = await GetOrCreateCartAsync(cancellationToken);
        return Ok(ToDto(cart));
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItem(
        AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            return BadRequest(new ApiMessage("Quantity must be greater than zero."));
        }

        var product = await _db.Products
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.IsActive, cancellationToken);

        if (product is null)
        {
            return NotFound(new ApiMessage("Product was not found."));
        }

        var available = product.Inventory?.QuantityAvailable ?? 0;
        if (available < request.Quantity)
        {
            return BadRequest(new ApiMessage("Not enough stock is available."));
        }

        var cart = await GetOrCreateCartAsync(cancellationToken);
        var existing = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);

        if (existing is null)
        {
            cart.Items.Add(new CartItem
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity
            });
        }
        else
        {
            if (existing.Quantity + request.Quantity > available)
            {
                return BadRequest(new ApiMessage("Cart quantity exceeds available stock."));
            }

            existing.Quantity += request.Quantity;
        }

        cart.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        cart = (await LoadCartAsync(cancellationToken))!;
        return Ok(ToDto(cart));
    }

    [HttpPut("items/{productId:guid}")]
    public async Task<ActionResult<CartDto>> UpdateItem(
        Guid productId,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var cart = await GetOrCreateCartAsync(cancellationToken);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
        {
            return NotFound(new ApiMessage("Cart item was not found."));
        }

        if (request.Quantity <= 0)
        {
            _db.CartItems.Remove(item);
        }
        else
        {
            var available = item.Product?.Inventory?.QuantityAvailable ?? 0;
            if (request.Quantity > available)
            {
                return BadRequest(new ApiMessage("Cart quantity exceeds available stock."));
            }

            item.Quantity = request.Quantity;
        }

        cart.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        cart = (await LoadCartAsync(cancellationToken))!;
        return Ok(ToDto(cart));
    }

    [HttpDelete("items/{productId:guid}")]
    public async Task<ActionResult<CartDto>> RemoveItem(Guid productId, CancellationToken cancellationToken)
    {
        var cart = await GetOrCreateCartAsync(cancellationToken);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
        {
            return NotFound(new ApiMessage("Cart item was not found."));
        }

        _db.CartItems.Remove(item);
        cart.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        cart = (await LoadCartAsync(cancellationToken))!;
        return Ok(ToDto(cart));
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        var cart = await GetOrCreateCartAsync(cancellationToken);
        _db.CartItems.RemoveRange(cart.Items);
        cart.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new ApiMessage("Cart cleared successfully."));
    }

    private async Task<Cart> GetOrCreateCartAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var cart = await LoadCartAsync(cancellationToken);

        if (cart is not null)
        {
            return cart;
        }

        cart = new Cart { UserId = userId };
        _db.Carts.Add(cart);
        await _db.SaveChangesAsync(cancellationToken);

        return (await LoadCartAsync(cancellationToken))!;
    }

    private async Task<Cart?> LoadCartAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        return await _db.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p!.Images)
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p!.Inventory)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    private static CartDto ToDto(Cart cart)
    {
        var now = DateTimeOffset.UtcNow;
        var items = cart.Items
            .OrderBy(i => i.CreatedAt)
            .Where(i => i.Product is not null)
            .Select(i =>
            {
                var product = i.Product!;
                var image = product.Images
                    .OrderByDescending(p => p.IsPrimary)
                    .ThenBy(p => p.SortOrder)
                    .FirstOrDefault();
                var unitPrice = product.GetEffectivePrice(now);

                return new CartItemDto(
                    product.Id,
                    product.Name,
                    product.Slug,
                    image?.Url,
                    unitPrice,
                    i.Quantity,
                    unitPrice * i.Quantity);
            })
            .ToList();

        return new CartDto(
            cart.Id,
            items,
            items.Sum(i => i.Quantity),
            items.Sum(i => i.LineTotal));
    }
}
