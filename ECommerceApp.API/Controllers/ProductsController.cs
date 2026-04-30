using ECommerceApp.API.Dtos;
using ECommerceApp.API.Data;
using ECommerceApp.API.Extensions;
using ECommerceApp.API.Models;
using ECommerceApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ECommerceApp.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _fileStorage;
    private readonly CurrentUserService _currentUser;

    public ProductsController(
        AppDbContext db,
        IFileStorageService fileStorage,
        CurrentUserService currentUser)
    {
        _db = db;
        _fileStorage = fileStorage;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetProducts(
        [FromQuery] ProductQueryParameters query,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);
        var now = DateTimeOffset.UtcNow;

        var productsQuery = _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .Include(p => p.Images)
            .Include(p => p.Reviews.Where(r => r.IsApproved))
            .AsQueryable();

        if (!query.IncludeInactive || !_currentUser.IsAdmin)
        {
            productsQuery = productsQuery.Where(p => p.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            productsQuery = productsQuery.Where(p =>
                EF.Functions.Like(p.Name, $"%{search}%") ||
                EF.Functions.Like(p.Description, $"%{search}%") ||
                EF.Functions.Like(p.Sku, $"%{search}%"));
        }

        if (query.CategoryId.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.CategoryId == query.CategoryId.Value);
        }

        if (query.MinPrice.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.Price >= query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.Price <= query.MaxPrice.Value);
        }

        productsQuery = query.Sort?.ToLowerInvariant() switch
        {
            "price_asc" or "price-asc" => productsQuery.OrderBy(p => p.Price),
            "price_desc" or "price-desc" => productsQuery.OrderByDescending(p => p.Price),
            "name" => productsQuery.OrderBy(p => p.Name),
            "featured" => productsQuery.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.CreatedAt),
            _ => productsQuery.OrderByDescending(p => p.CreatedAt)
        };

        var total = await productsQuery.CountAsync(cancellationToken);
        var products = await productsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Ok(new PagedResult<ProductListItemDto>(
            products.Select(p => p.ToListDto(now)).ToList(),
            page,
            pageSize,
            total));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDetailsDto>> GetProduct(Guid id, CancellationToken cancellationToken)
    {
        var product = await ProductDetailsQuery()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product is null || (!product.IsActive && !_currentUser.IsAdmin))
        {
            return NotFound(new ApiMessage("Product was not found."));
        }

        return Ok(product.ToDetailsDto(DateTimeOffset.UtcNow));
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ProductDetailsDto>> GetProductBySlug(string slug, CancellationToken cancellationToken)
    {
        var product = await ProductDetailsQuery()
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);

        if (product is null || (!product.IsActive && !_currentUser.IsAdmin))
        {
            return NotFound(new ApiMessage("Product was not found."));
        }

        return Ok(product.ToDetailsDto(DateTimeOffset.UtcNow));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [RequestSizeLimit(6_000_000)]
    public async Task<ActionResult<ProductDetailsDto>> CreateProduct(
        [FromForm] ProductUpsertRequest request,
        CancellationToken cancellationToken)
    {
        await ValidateCategoryAsync(request.CategoryId, cancellationToken);

        var slug = NormalizeSlug(request.Slug, request.Name);
        var sku = NormalizeSku(request.Sku, request.Name);

        if (await _db.Products.AnyAsync(p => p.Slug == slug, cancellationToken))
        {
            return Conflict(new ApiMessage("A product with this slug already exists."));
        }

        if (await _db.Products.AnyAsync(p => p.Sku == sku, cancellationToken))
        {
            return Conflict(new ApiMessage("A product with this SKU already exists."));
        }

        var imageUrl = await _fileStorage.SaveProductImageAsync(request.ImageFile, cancellationToken);
        var product = new Product
        {
            Name = request.Name.Trim(),
            Slug = slug,
            Description = request.Description.Trim(),
            Brand = request.Brand?.Trim(),
            Sku = sku,
            Price = request.Price,
            CompareAtPrice = request.CompareAtPrice,
            DiscountPercent = request.DiscountPercent,
            DiscountStartAt = request.DiscountStartAt,
            DiscountEndAt = request.DiscountEndAt,
            CategoryId = request.CategoryId,
            IsActive = request.IsActive,
            IsFeatured = request.IsFeatured,
            Inventory = new InventoryItem
            {
                QuantityAvailable = request.QuantityAvailable,
                LowStockThreshold = request.LowStockThreshold,
                LastRestockedAt = request.QuantityAvailable > 0 ? DateTimeOffset.UtcNow : null
            }
        };

        if (imageUrl is not null)
        {
            product.Images.Add(new ProductImage
            {
                Url = imageUrl,
                AltText = product.Name,
                IsPrimary = true
            });
        }

        _db.Products.Add(product);
        await _db.SaveChangesAsync(cancellationToken);

        var created = await ProductDetailsQuery()
            .FirstAsync(p => p.Id == product.Id, cancellationToken);

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, created.ToDetailsDto(DateTimeOffset.UtcNow));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    [RequestSizeLimit(6_000_000)]
    public async Task<ActionResult<ProductDetailsDto>> UpdateProduct(
        Guid id,
        [FromForm] ProductUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _db.Products
            .Include(p => p.Inventory)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product is null)
        {
            return NotFound(new ApiMessage("Product was not found."));
        }

        await ValidateCategoryAsync(request.CategoryId, cancellationToken);

        var slug = NormalizeSlug(request.Slug, request.Name);
        var sku = NormalizeSku(request.Sku, request.Name);

        if (await _db.Products.AnyAsync(p => p.Id != id && p.Slug == slug, cancellationToken))
        {
            return Conflict(new ApiMessage("A product with this slug already exists."));
        }

        if (await _db.Products.AnyAsync(p => p.Id != id && p.Sku == sku, cancellationToken))
        {
            return Conflict(new ApiMessage("A product with this SKU already exists."));
        }

        product.Name = request.Name.Trim();
        product.Slug = slug;
        product.Description = request.Description.Trim();
        product.Brand = request.Brand?.Trim();
        product.Sku = sku;
        product.Price = request.Price;
        product.CompareAtPrice = request.CompareAtPrice;
        product.DiscountPercent = request.DiscountPercent;
        product.DiscountStartAt = request.DiscountStartAt;
        product.DiscountEndAt = request.DiscountEndAt;
        product.CategoryId = request.CategoryId;
        product.IsActive = request.IsActive;
        product.IsFeatured = request.IsFeatured;
        product.UpdatedAt = DateTimeOffset.UtcNow;

        product.Inventory ??= new InventoryItem { ProductId = product.Id };
        product.Inventory.QuantityAvailable = request.QuantityAvailable;
        product.Inventory.LowStockThreshold = request.LowStockThreshold;
        product.Inventory.LastRestockedAt = request.QuantityAvailable > 0 ? DateTimeOffset.UtcNow : product.Inventory.LastRestockedAt;

        var imageUrl = await _fileStorage.SaveProductImageAsync(request.ImageFile, cancellationToken);
        if (imageUrl is not null)
        {
            foreach (var image in product.Images)
            {
                image.IsPrimary = false;
            }

            product.Images.Add(new ProductImage
            {
                Url = imageUrl,
                AltText = product.Name,
                IsPrimary = true
            });
        }

        await _db.SaveChangesAsync(cancellationToken);

        var updated = await ProductDetailsQuery()
            .FirstAsync(p => p.Id == product.Id, cancellationToken);

        return Ok(updated.ToDetailsDto(DateTimeOffset.UtcNow));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product is null)
        {
            return NotFound(new ApiMessage("Product was not found."));
        }

        product.IsActive = false;
        product.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new ApiMessage("Product deactivated successfully."));
    }

    private IQueryable<Product> ProductDetailsQuery()
    {
        return _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .Include(p => p.Images)
            .Include(p => p.Reviews.Where(r => r.IsApproved));
    }

    private async Task ValidateCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        var exists = await _db.Categories.AnyAsync(c => c.Id == categoryId && c.IsActive, cancellationToken);
        if (!exists)
        {
            throw new ArgumentException("Category does not exist or is inactive.");
        }
    }

    private static string NormalizeSlug(string? slug, string name)
    {
        var value = string.IsNullOrWhiteSpace(slug) ? name : slug;
        value = Regex.Replace(value.Trim().ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString("N")[..12] : value;
    }

    private static string NormalizeSku(string? sku, string name)
    {
        if (!string.IsNullOrWhiteSpace(sku))
        {
            return sku.Trim().ToUpperInvariant();
        }

        var prefix = Regex.Replace(name.ToUpperInvariant(), "[^A-Z0-9]", "");
        prefix = prefix.Length >= 4 ? prefix[..4] : prefix.PadRight(4, 'X');
        return $"{prefix}-{Random.Shared.Next(1000, 9999)}";
    }
}
