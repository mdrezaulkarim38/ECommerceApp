namespace ECommerceApp.API.Dtos;

public record InventoryDto(
    Guid ProductId,
    int QuantityAvailable,
    int QuantityReserved,
    int LowStockThreshold,
    bool IsLowStock,
    DateTimeOffset? LastRestockedAt);

public record InventoryUpdateRequest(
    int QuantityAvailable,
    int LowStockThreshold);
