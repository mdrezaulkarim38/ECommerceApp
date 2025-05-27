using Microsoft.AspNetCore.Mvc;
using ECommerceApp.API.Dtos;
using ECommerceApp.API.Interfaces;

namespace ECommerceApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(AddToCartRequest request)
    {
        var result = await _cartService.AddToCartAsync(request);
        return Ok(result);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCart(string userId)
    {
        var result = await _cartService.GetCartAsync(userId);
        return Ok(result);
    }

    [HttpDelete("{userId}/{productId}")]
    public async Task<IActionResult> RemoveFromCart(string userId, int productId)
    {
        var success = await _cartService.RemoveFromCartAsync(userId, productId);
        if (!success)
            return NotFound("Cart item not found");
        return Ok();
    }

    [HttpPut("{userId}/{productId}")]
    public async Task<IActionResult> UpdateCartItem(string userId, int productId, [FromBody] UpdateCartItemRequest request)
    {
        var result = await _cartService.UpdateCartItemAsync(userId, productId, request);
        if (result == null)
            return NotFound("Cart item not found");

        return Ok(result);
    }
}
