using ECommerceApp.API.Dtos;

namespace ECommerceApp.API.Interfaces;

public interface ICartService
{
    Task<object> AddToCartAsync(AddToCartRequest request);
    Task<object> GetCartAsync(string userId);
    Task<bool> RemoveFromCartAsync(string userId, int productId);
    Task<object?> UpdateCartItemAsync(string userId, int productId, UpdateCartItemRequest request);
}
