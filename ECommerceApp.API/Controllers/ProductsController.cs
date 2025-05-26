using Microsoft.AspNetCore.Mvc;
using ECommerceApp.API.Data;
using ECommerceApp.API.Models;
using ECommerceApp.API.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromForm] ProductDto productDto)
    {
        string? imageUrl = null;
        if (productDto.ImageFile != null)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", productDto.ImageFile.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await productDto.ImageFile.CopyToAsync(stream);
            }
            imageUrl = $"/images/{productDto.ImageFile.FileName}";
        }

        var product = new Product
        {
            Name = productDto.Name,
            Slug = productDto.Slug,
            Price = productDto.Price,
            DiscountPercent = productDto.DiscountPercent,
            DiscountStartDate = productDto.DiscountStartDate,
            DiscountEndDate = productDto.DiscountEndDate,
            ImageUrl = imageUrl
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(string? search, int page = 1, int pageSize = 8)
    {
        var query = _context.Products.AsQueryable();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name!.Contains(search));

        var total = await query.CountAsync();
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var today = DateTime.Today;
        var productDtos = products.Select(p => new
        {
            p.Id,
            p.Name,
            p.Slug,
            p.Price,
            DiscountedPrice = (p.DiscountPercent.HasValue && p.DiscountStartDate <= today && today <= p.DiscountEndDate)
                ? p.Price * (1 - p.DiscountPercent.Value / 100)
                : (decimal?)null,
            p.ImageUrl
        }).ToList();

        return Ok(new { total, products = productDtos });
    }
}

