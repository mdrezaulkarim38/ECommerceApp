using ECommerceApp.API.Data;
using ECommerceApp.API.Dtos;
using ECommerceApp.API.Extensions;
using ECommerceApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ECommerceApp.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new
            {
                Category = c,
                ProductCount = c.Products.Count(p => p.IsActive)
            })
            .ToListAsync(cancellationToken);

        return Ok(categories.Select(c => c.Category.ToDto(c.ProductCount)).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(Guid id, CancellationToken cancellationToken)
    {
        var category = await _db.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new
            {
                Category = c,
                ProductCount = c.Products.Count(p => p.IsActive)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            return NotFound(new ApiMessage("Category was not found."));
        }

        return Ok(category.Category.ToDto(category.ProductCount));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(
        CategoryUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var slug = NormalizeSlug(request.Slug, request.Name);
        if (await _db.Categories.AnyAsync(c => c.Slug == slug, cancellationToken))
        {
            return Conflict(new ApiMessage("A category with this slug already exists."));
        }

        var category = new Category
        {
            Name = request.Name.Trim(),
            Slug = slug,
            Description = request.Description?.Trim(),
            IsActive = request.IsActive
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category.ToDto());
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(
        Guid id,
        CategoryUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (category is null)
        {
            return NotFound(new ApiMessage("Category was not found."));
        }

        var slug = NormalizeSlug(request.Slug, request.Name);
        if (await _db.Categories.AnyAsync(c => c.Id != id && c.Slug == slug, cancellationToken))
        {
            return Conflict(new ApiMessage("A category with this slug already exists."));
        }

        category.Name = request.Name.Trim();
        category.Slug = slug;
        category.Description = request.Description?.Trim();
        category.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);

        var productCount = await _db.Products.CountAsync(p => p.CategoryId == category.Id && p.IsActive, cancellationToken);
        return Ok(category.ToDto(productCount));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (category is null)
        {
            return NotFound(new ApiMessage("Category was not found."));
        }

        category.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new ApiMessage("Category deactivated successfully."));
    }

    private static string NormalizeSlug(string? slug, string name)
    {
        var value = string.IsNullOrWhiteSpace(slug) ? name : slug;
        value = Regex.Replace(value.Trim().ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString("N")[..12] : value;
    }
}
