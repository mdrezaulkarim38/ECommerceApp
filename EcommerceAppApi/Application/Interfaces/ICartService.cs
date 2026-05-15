using EcommerceAppApi.Application.DTOs;

namespace EcommerceAppApi.Application.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(int userId);
    Task<CartDto> AddToCartAsync(int userId, AddToCartRequest request);
    Task<CartDto> UpdateCartItemAsync(int userId, int productId, UpdateCartItemRequest request);
    Task<CartDto> RemoveFromCartAsync(int userId, int productId);
    Task ClearCartAsync(int userId);
    Task<int> GetCartCountAsync(int userId);
}
