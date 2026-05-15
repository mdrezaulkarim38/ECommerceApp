using EcommerceAppApi.Application.DTOs;

namespace EcommerceAppApi.Application.Interfaces;

public interface IWishlistService
{
    Task<List<ProductDto>> GetWishlistAsync(int userId);
    Task<bool> ToggleWishlistAsync(int userId, int productId);
    Task<bool> IsInWishlistAsync(int userId, int productId);
}
