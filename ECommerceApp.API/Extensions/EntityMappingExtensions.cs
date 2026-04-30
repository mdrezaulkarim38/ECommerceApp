using ECommerceApp.API.Dtos;
using ECommerceApp.API.Models;

namespace ECommerceApp.API.Extensions;

public static class EntityMappingExtensions
{
    public static CategoryDto ToDto(this Category category, int productCount = 0)
    {
        return new CategoryDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.IsActive,
            productCount);
    }

    public static ProductListItemDto ToListDto(this Product product, DateTimeOffset now)
    {
        var primaryImage = product.Images
            .OrderByDescending(i => i.IsPrimary)
            .ThenBy(i => i.SortOrder)
            .FirstOrDefault();

        return new ProductListItemDto(
            product.Id,
            product.Name,
            product.Slug,
            product.Brand,
            product.Sku,
            product.Price,
            product.GetEffectivePrice(now),
            product.DiscountPercent,
            product.Category?.Name ?? string.Empty,
            primaryImage?.Url,
            product.Inventory?.QuantityAvailable ?? 0,
            product.IsActive,
            product.IsFeatured,
            product.Reviews.Count == 0 ? 0 : Math.Round(product.Reviews.Average(r => r.Rating), 1));
    }

    public static ProductDetailsDto ToDetailsDto(this Product product, DateTimeOffset now)
    {
        var inventory = product.Inventory ?? new InventoryItem { ProductId = product.Id };
        var reviews = product.Reviews.Where(r => r.IsApproved).ToList();

        return new ProductDetailsDto(
            product.Id,
            product.Name,
            product.Slug,
            product.Description,
            product.Brand,
            product.Sku,
            product.Price,
            product.GetEffectivePrice(now),
            product.CompareAtPrice,
            product.DiscountPercent,
            product.DiscountStartAt,
            product.DiscountEndAt,
            product.Category?.ToDto() ?? new CategoryDto(Guid.Empty, string.Empty, string.Empty, null, false, 0),
            product.Images
                .OrderByDescending(i => i.IsPrimary)
                .ThenBy(i => i.SortOrder)
                .Select(i => new ProductImageDto(i.Id, i.Url, i.AltText, i.IsPrimary, i.SortOrder))
                .ToList(),
            new InventoryDto(
                inventory.ProductId,
                inventory.QuantityAvailable,
                inventory.QuantityReserved,
                inventory.LowStockThreshold,
                inventory.QuantityAvailable <= inventory.LowStockThreshold,
                inventory.LastRestockedAt),
            product.IsActive,
            product.IsFeatured,
            reviews.Count == 0 ? 0 : Math.Round(reviews.Average(r => r.Rating), 1),
            reviews.Count);
    }

    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto(
            order.Id,
            order.OrderNumber,
            order.Status,
            order.PaymentStatus,
            order.PaymentMethod,
            order.Subtotal,
            order.DiscountTotal,
            order.ShippingTotal,
            order.TaxTotal,
            order.Total,
            order.ShippingName,
            order.ShippingPhone,
            order.ShippingAddressLine1,
            order.ShippingCity,
            order.ShippingCountry,
            order.Notes,
            order.CreatedAt,
            order.Items
                .Select(i => new OrderItemDto(
                    i.ProductId,
                    i.ProductName,
                    i.Sku,
                    i.UnitPrice,
                    i.DiscountAmount,
                    i.Quantity,
                    i.LineTotal))
                .ToList());
    }
}
