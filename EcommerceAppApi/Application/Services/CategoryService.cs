using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using EcommerceAppApi.Domain.Entities;
using EcommerceAppApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAppApi.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    public CategoryService(ApplicationDbContext context) => _context = context;

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        return await _context.Categories
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                ParentCategoryId = c.ParentCategoryId,
                DisplayOrder = c.DisplayOrder,
                ProductCount = c.Products.Count(p => p.IsActive)
            })
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                ParentCategoryId = c.ParentCategoryId,
                DisplayOrder = c.DisplayOrder,
                ProductCount = c.Products.Count(p => p.IsActive)
            })
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name,
            Slug = GenerateSlug(request.Name),
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            ParentCategoryId = request.ParentCategoryId,
            DisplayOrder = request.DisplayOrder
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            ParentCategoryId = category.ParentCategoryId,
            DisplayOrder = category.DisplayOrder,
            ProductCount = 0
        };
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = await _context.Categories.FindAsync(id)
            ?? throw new KeyNotFoundException("Category not found");

        category.Name = request.Name;
        category.Slug = GenerateSlug(request.Name);
        category.Description = request.Description;
        category.ImageUrl = request.ImageUrl;
        category.ParentCategoryId = request.ParentCategoryId;
        category.DisplayOrder = request.DisplayOrder;

        await _context.SaveChangesAsync();

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            ParentCategoryId = category.ParentCategoryId,
            DisplayOrder = category.DisplayOrder,
            ProductCount = await _context.Products.CountAsync(p => p.CategoryId == id && p.IsActive)
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.SubCategories)
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return false;

        if (category.Products.Any(p => p.IsActive))
            throw new InvalidOperationException("Cannot delete category with active products");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    private static string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant().Trim();
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
        return slug;
    }
}
