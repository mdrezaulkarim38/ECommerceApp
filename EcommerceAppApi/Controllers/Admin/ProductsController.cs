using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAppApi.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    public ProductsController(IProductService productService) => _productService = productService;

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductRequest request)
    {
        var product = await _productService.CreateProductAsync(request);
        return Ok(ApiResponse<ProductDto>.Ok(product, "Product created"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            var product = await _productService.UpdateProductAsync(id, request);
            return Ok(ApiResponse<ProductDto>.Ok(product, "Product updated"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ProductDto>.Error(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        if (!result) return NotFound(ApiResponse<string>.Error("Product not found"));
        return Ok(ApiResponse<string>.Ok("", "Product deleted"));
    }
}
