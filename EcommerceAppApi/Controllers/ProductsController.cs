using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAppApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;
    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ProductDto>>>> GetProducts(
        [FromQuery] ProductListRequest request)
    {
        var result = await _productService.GetProductsAsync(request);
        _logger.LogInformation("Products listed: page {Page}, {Count} items", request.Page, result.Items.Count());
        return Ok(ApiResponse<PaginatedResponse<ProductDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDetailDto>>> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound(ApiResponse<ProductDetailDto>.Error("Product not found"));
        return Ok(ApiResponse<ProductDetailDto>.Ok(product));
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ApiResponse<ProductDetailDto>>> GetProductBySlug(string slug)
    {
        var product = await _productService.GetProductBySlugAsync(slug);
        if (product == null) return NotFound(ApiResponse<ProductDetailDto>.Error("Product not found"));
        return Ok(ApiResponse<ProductDetailDto>.Ok(product));
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        return Ok(ApiResponse<List<ProductDto>>.Ok(products));
    }

    [HttpGet("brand/{brandId}")]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetByBrand(int brandId)
    {
        var products = await _productService.GetProductsByBrandAsync(brandId);
        return Ok(ApiResponse<List<ProductDto>>.Ok(products));
    }
}
