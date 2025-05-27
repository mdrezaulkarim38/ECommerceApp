using ECommerceApp.API.Data;
using ECommerceApp.API.Dtos;
using ECommerceApp.API.Interfaces;
using ECommerceApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.API.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<object> AddProductAsync(ProductDto productDto)
    {
        if (!ValidateProductDto(productDto, out var validationError))
            return new { Error = validationError };

        string? imageUrl = null;
        if (productDto.ImageFile != null)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(productDto.ImageFile.FileName);
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

        return new
        {
            product.Id,
            product.Name,
            product.Slug,
            product.Price,
            DiscountedPrice = product.DiscountPercent.HasValue &&
                              product.DiscountStartDate <= DateTime.Today &&
                              DateTime.Today <= product.DiscountEndDate
                ? product.Price * (1 - product.DiscountPercent.Value / 100)
                : (decimal?)null,
            product.ImageUrl
        };
    }

    public async Task<object> GetProductsAsync(string? search, int page, int pageSize)
    {
        var query = _context.Products.AsNoTracking().AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => EF.Functions.Like(p.Name, $"%{search}%"));
        }

        var total = await query.CountAsync();
        var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var today = DateTime.Today;

        var productDtos = products.Select(p => new
        {
            p.Id,
            p.Name,
            p.Slug,
            p.Price,
            DiscountedPrice = p.DiscountPercent.HasValue &&
                              p.DiscountStartDate.HasValue &&
                              p.DiscountEndDate.HasValue &&
                              p.DiscountStartDate.Value <= today &&
                              today <= p.DiscountEndDate.Value
                ? p.Price * (1 - p.DiscountPercent.Value / 100)
                : (decimal?)null,
            p.ImageUrl
        }).ToList();

        return new { Total = total, Products = productDtos };
    }

    public async Task<object> GetDashboardStatsAsync()
    {
        var totalProducts = await _context.Products.CountAsync();
        var totalVendors = 10;
        var uniqueProducts = await _context.Products
            .GroupBy(p => p.Name)
            .CountAsync();

        return new
        {
            TotalProducts = totalProducts,
            TotalVendors = totalVendors,
            UniqueProducts = uniqueProducts
        };
    }

    private bool ValidateProductDto(ProductDto dto, out string? error)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Slug))
        {
            error = "Name and Slug are required.";
            return false;
        }
        error = null;
        return true;
    }
}
