using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAppApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public BrandsController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetBrands()
    {
        var brands = await _context.Brands
            .Select(b => new
            {
                b.Id,
                b.Name,
                b.Slug,
                b.LogoUrl,
                b.Description,
                b.Website,
                ProductCount = b.Products.Count(p => p.IsActive),
                AverageRating = b.Products.Where(p => p.IsActive).Average(p => (double?)p.AverageRating) ?? 0
            })
            .ToListAsync();

        return Ok(ApiResponse<List<object>>.Ok(brands.Cast<object>().ToList()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetBrand(int id)
    {
        var brand = await _context.Brands
            .Select(b => new
            {
                b.Id,
                b.Name,
                b.Slug,
                b.LogoUrl,
                b.Description,
                b.Website,
                ProductCount = b.Products.Count(p => p.IsActive),
                AverageRating = b.Products.Where(p => p.IsActive).Average(p => (double?)p.AverageRating) ?? 0
            })
            .FirstOrDefaultAsync(b => b.Id == id);

        if (brand == null) return NotFound(ApiResponse<object>.Error("Brand not found"));
        return Ok(ApiResponse<object>.Ok(brand));
    }
}
