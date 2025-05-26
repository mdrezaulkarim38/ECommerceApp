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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        string? imageUrl = null;
        if (productDto.ImageFile != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(productDto.ImageFile.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!); 
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await productDto.ImageFile.CopyToAsync(stream);
            }
            imageUrl = $"/images/{fileName}";
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
        return Ok(new
        {
            product.Id,
            product.Name,
            product.Slug,
            product.Price,
            DiscountedPrice = product.DiscountPercent.HasValue && product.DiscountStartDate <= DateTime.Today && DateTime.Today <= product.DiscountEndDate
                ? product.Price * (1 - product.DiscountPercent.Value / 100)
                : (decimal?)null,
            product.ImageUrl
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 8)
    {
        if (page < 1 || pageSize < 1)
            return BadRequest("Page and pageSize must be greater than 0");

        var query = _context.Products.AsNoTracking().AsQueryable();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name!.Contains(search, StringComparison.OrdinalIgnoreCase));

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
            DiscountedPrice = p.DiscountPercent.HasValue && p.DiscountStartDate.HasValue && p.DiscountEndDate.HasValue
                && p.DiscountStartDate.Value <= today && today <= p.DiscountEndDate.Value
                ? p.Price * (1 - p.DiscountPercent.Value / 100)
                : (decimal?)null,
            p.ImageUrl
        }).ToList();

        return Ok(new { Total = total, Products = productDtos });
    }
}