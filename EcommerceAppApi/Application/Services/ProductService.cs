using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using EcommerceAppApi.Domain.Entities;
using EcommerceAppApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAppApi.Application.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    public ProductService(ApplicationDbContext context) => _context = context;

    public async Task<PaginatedResponse<ProductDto>> GetProductsAsync(ProductListRequest request)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(p => p.Name.Contains(request.Search) || p.Description.Contains(request.Search));

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId);

        if (request.BrandId.HasValue)
            query = query.Where(p => p.BrandId == request.BrandId);

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.Price >= request.MinPrice.Value);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= request.MaxPrice.Value);

        if (request.MinRating.HasValue)
            query = query.Where(p => p.AverageRating >= request.MinRating.Value);

        query = (request.SortBy?.ToLower()) switch
        {
            "price" => request.SortOrder == "desc" ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "name" => request.SortOrder == "desc" ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "rating" => query.OrderByDescending(p => p.AverageRating ?? 0),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            "popular" => query.OrderByDescending(p => p.TotalReviews),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => MapToDto(p))
            .ToListAsync();

        return new PaginatedResponse<ProductDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize)
        };
    }

    public async Task<ProductDetailDto?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
            .FirstOrDefaultAsync(p => p.Id == id);

        return product == null ? null : MapToDetailDto(product);
    }

    public async Task<ProductDetailDto?> GetProductBySlugAsync(string slug)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
            .FirstOrDefaultAsync(p => p.Slug == slug);

        return product == null ? null : MapToDetailDto(product);
    }

    public async Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<List<ProductDto>> GetProductsByBrandAsync(int brandId)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.BrandId == brandId && p.IsActive)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Slug = request.Name.ToLower().Replace(" ", "-"),
            Description = request.Description,
            Price = request.Price,
            CompareAtPrice = request.CompareAtPrice,
            StockQuantity = request.StockQuantity,
            CategoryId = request.CategoryId,
            BrandId = request.BrandId,
            MainImageUrl = request.MainImageUrl,
            Sku = request.Sku,
            IsFeatured = request.IsFeatured,
            IsActive = true,
            SpecsJson = request.Specs,
            FeaturesJson = request.Features,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) throw new KeyNotFoundException("Product not found");

        product.Name = request.Name;
        product.Slug = request.Name.ToLower().Replace(" ", "-");
        product.Description = request.Description;
        product.Price = request.Price;
        product.CompareAtPrice = request.CompareAtPrice;
        product.StockQuantity = request.StockQuantity;
        product.CategoryId = request.CategoryId;
        product.BrandId = request.BrandId;
        product.MainImageUrl = request.MainImageUrl;
        product.Sku = request.Sku;
        product.IsActive = request.IsActive;
        product.IsFeatured = request.IsFeatured;
        product.SpecsJson = request.Specs;
        product.FeaturesJson = request.Features;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private static ProductDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Slug = p.Slug,
        Description = p.Description,
        Price = p.Price,
        CompareAtPrice = p.CompareAtPrice,
        StockQuantity = p.StockQuantity,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name,
        BrandId = p.BrandId,
        BrandName = p.Brand?.Name,
        MainImageUrl = p.MainImageUrl,
        AverageRating = p.AverageRating,
        TotalReviews = p.TotalReviews,
        SalesCount = p.SalesCount,
        IsActive = p.IsActive
    };

    private static ProductDetailDto MapToDetailDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Slug = p.Slug,
        Description = p.Description,
        Price = p.Price,
        CompareAtPrice = p.CompareAtPrice,
        StockQuantity = p.StockQuantity,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name,
        BrandId = p.BrandId,
        BrandName = p.Brand?.Name,
        MainImageUrl = p.MainImageUrl,
        AverageRating = p.AverageRating,
        TotalReviews = p.TotalReviews,
        SalesCount = p.SalesCount,
        IsActive = p.IsActive,
        Specs = p.SpecsJson,
        Features = p.FeaturesJson,
        Images = p.Images.Select(i => new ProductImageDto
        {
            Id = i.Id,
            ImageUrl = i.ImageUrl,
            AltText = i.AltText,
            DisplayOrder = i.DisplayOrder
        }).ToList()
    };
}
