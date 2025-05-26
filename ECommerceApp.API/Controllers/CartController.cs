using Microsoft.AspNetCore.Mvc;
using ECommerceApp.API.Data;
using ECommerceApp.API.Models;
using ECommerceApp.API.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<IActionResult> AddToCart(AddToCartRequest request)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == request.UserId && ci.ProductId == request.ProductId);

        if (cartItem == null)
        {
            cartItem = new CartItem
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
        return Ok(cartItem);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCart(string userId)
    {
        var cart = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return Ok(cart);
    }
}