using Microsoft.AspNetCore.Mvc;
using ECommerceApp.API.Data;
using ECommerceApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    public ProductsController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<IActionResult> AddProduct(Product product)
    {
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

        return Ok(new { total, products });
    }
}