using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAppApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public CategoriesController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetCategories()
    {
        var categories = await _context.Categories
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.ImageUrl,
                c.ParentCategoryId,
                c.DisplayOrder,
                ProductCount = c.Products.Count(p => p.IsActive)
            })
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        return Ok(ApiResponse<List<object>>.Ok(categories.Cast<object>().ToList()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetCategory(int id)
    {
        var category = await _context.Categories
            .Include(c => c.SubCategories)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.ImageUrl,
                c.ParentCategoryId,
                c.DisplayOrder,
                ProductCount = c.Products.Count(p => p.IsActive),
                SubCategories = c.SubCategories.Select(s => new { s.Id, s.Name, s.Slug })
            })
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return NotFound(ApiResponse<object>.Error("Category not found"));
        return Ok(ApiResponse<object>.Ok(category));
    }
}
