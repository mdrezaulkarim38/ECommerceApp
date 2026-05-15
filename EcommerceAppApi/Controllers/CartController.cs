using System.Security.Claims;
using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAppApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    public CartController(ICartService cartService) => _cartService = cartService;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<ApiResponse<CartDto>>> GetCart()
    {
        var cart = await _cartService.GetCartAsync(UserId);
        return Ok(ApiResponse<CartDto>.Ok(cart));
    }

    [HttpGet("count")]
    public async Task<ActionResult<ApiResponse<int>>> GetCount()
    {
        var count = await _cartService.GetCartCountAsync(UserId);
        return Ok(ApiResponse<int>.Ok(count));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CartDto>>> AddToCart([FromBody] AddToCartRequest request)
    {
        var cart = await _cartService.AddToCartAsync(UserId, request);
        return Ok(ApiResponse<CartDto>.Ok(cart, "Item added to cart"));
    }

    [HttpPut("{productId}")]
    public async Task<ActionResult<ApiResponse<CartDto>>> UpdateCartItem(int productId, [FromBody] UpdateCartItemRequest request)
    {
        try
        {
            var cart = await _cartService.UpdateCartItemAsync(UserId, productId, request);
            return Ok(ApiResponse<CartDto>.Ok(cart));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CartDto>.Error(ex.Message));
        }
    }

    [HttpDelete("{productId}")]
    public async Task<ActionResult<ApiResponse<CartDto>>> RemoveFromCart(int productId)
    {
        var cart = await _cartService.RemoveFromCartAsync(UserId, productId);
        return Ok(ApiResponse<CartDto>.Ok(cart, "Item removed from cart"));
    }

    [HttpDelete]
    public async Task<ActionResult<ApiResponse<string>>> ClearCart()
    {
        await _cartService.ClearCartAsync(UserId);
        return Ok(ApiResponse<string>.Ok("", "Cart cleared"));
    }
}
