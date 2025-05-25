using Microsoft.AspNetCore.Mvc;
using ECommerceApp.API.Data;
using ECommerceApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;
    public CartController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<IActionResult> AddToCart(CartItem item)
    {
        _context.CartItems.Add(item);
        await _context.SaveChangesAsync();
        return Ok(item);
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var cart = await _context.CartItems
            .Include(c => c.Product)
            .ToListAsync();

        return Ok(cart);
    }
}