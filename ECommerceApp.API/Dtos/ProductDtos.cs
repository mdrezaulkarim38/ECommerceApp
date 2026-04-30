using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.API.Dtos;

public class ProductQueryParameters
{
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Sort { get; set; } = "newest";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public bool IncludeInactive { get; set; }
}

public class ProductUpsertRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? Sku { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal? DiscountPercent { get; set; }
    public DateTimeOffset? DiscountStartAt { get; set; }
    public DateTimeOffset? DiscountEndAt { get; set; }
    public Guid CategoryId { get; set; }
    public int QuantityAvailable { get; set; }
    public int LowStockThreshold { get; set; } = 5;
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; }
    [FromForm]
    public IFormFile? ImageFile { get; set; }
}

public record ProductListItemDto(
    Guid Id,
    string Name,
    string Slug,
    string? Brand,
    string Sku,
    decimal Price,
    decimal EffectivePrice,
    decimal? DiscountPercent,
    string CategoryName,
    string? PrimaryImageUrl,
    int QuantityAvailable,
    bool IsActive,
    bool IsFeatured,
    double AverageRating);

public record ProductDetailsDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    string? Brand,
    string Sku,
    decimal Price,
    decimal EffectivePrice,
    decimal? CompareAtPrice,
    decimal? DiscountPercent,
    DateTimeOffset? DiscountStartAt,
    DateTimeOffset? DiscountEndAt,
    CategoryDto Category,
    IReadOnlyList<ProductImageDto> Images,
    InventoryDto Inventory,
    bool IsActive,
    bool IsFeatured,
    double AverageRating,
    int ReviewCount);

public record ProductImageDto(Guid Id, string Url, string? AltText, bool IsPrimary, int SortOrder);
