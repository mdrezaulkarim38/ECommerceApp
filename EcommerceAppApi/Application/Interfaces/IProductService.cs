using EcommerceAppApi.Application.DTOs;

namespace EcommerceAppApi.Application.Interfaces;

public interface IProductService
{
    Task<PaginatedResponse<ProductDto>> GetProductsAsync(ProductListRequest request);
    Task<ProductDetailDto?> GetProductByIdAsync(int id);
    Task<ProductDetailDto?> GetProductBySlugAsync(string slug);
    Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId);
    Task<List<ProductDto>> GetProductsByBrandAsync(int brandId);
    Task<ProductDto> CreateProductAsync(CreateProductRequest request);
    Task<ProductDto> UpdateProductAsync(int id, UpdateProductRequest request);
    Task<bool> DeleteProductAsync(int id);
}
