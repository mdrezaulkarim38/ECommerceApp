using Microsoft.AspNetCore.Mvc;
using ECommerceApp.API.Dtos;
using ECommerceApp.API.Interfaces;

namespace ECommerceApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromForm] ProductDto productDto)
    {
        var result = await _productService.AddProductAsync(productDto);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 8)
    {
        if (page < 1 || pageSize < 1)
            return BadRequest("Page and pageSize must be greater than 0");

        var result = await _productService.GetProductsAsync(search, page, pageSize);
        return Ok(result);
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var result = await _productService.GetDashboardStatsAsync();
        return Ok(result);
    }
}