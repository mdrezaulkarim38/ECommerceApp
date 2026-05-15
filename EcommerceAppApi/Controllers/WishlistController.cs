using System.Security.Claims;
using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAppApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;
    public WishlistController(IWishlistService wishlistService) => _wishlistService = wishlistService;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetWishlist()
    {
        var items = await _wishlistService.GetWishlistAsync(UserId);
        return Ok(ApiResponse<List<ProductDto>>.Ok(items));
    }

    [HttpPost("{productId}")]
    public async Task<ActionResult<ApiResponse<bool>>> ToggleWishlist(int productId)
    {
        var isInWishlist = await _wishlistService.ToggleWishlistAsync(UserId, productId);
        var message = isInWishlist ? "Added to wishlist" : "Removed from wishlist";
        return Ok(ApiResponse<bool>.Ok(isInWishlist, message));
    }

    [HttpGet("check/{productId}")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckWishlist(int productId)
    {
        var isInWishlist = await _wishlistService.IsInWishlistAsync(UserId, productId);
        return Ok(ApiResponse<bool>.Ok(isInWishlist));
    }
}
