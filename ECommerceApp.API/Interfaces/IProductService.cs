using ECommerceApp.API.Dtos;
using ECommerceApp.API.Models;

namespace ECommerceApp.API.Interfaces;

public interface IProductService
{
    Task<object> AddProductAsync(ProductDto productDto);
    Task<object> GetProductsAsync(string? search, int page, int pageSize);
    Task<object> GetDashboardStatsAsync();
}
