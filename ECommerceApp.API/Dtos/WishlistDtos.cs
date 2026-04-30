namespace ECommerceApp.API.Dtos;

public record WishlistItemDto(
    Guid ProductId,
    string ProductName,
    string Slug,
    string? ImageUrl,
    decimal Price,
    decimal EffectivePrice,
    DateTimeOffset AddedAt);
