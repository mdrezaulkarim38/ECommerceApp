namespace ECommerceApp.API.Dtos;

public record DashboardSummaryDto(
    int TotalProducts,
    int TotalCategories,
    int TotalCustomers,
    int TotalOrders,
    decimal RevenueLast30Days,
    int PendingOrders,
    int LowStockProducts,
    IReadOnlyList<TopProductDto> TopProducts,
    IReadOnlyList<LowStockProductDto> LowStockItems);

public record TopProductDto(
    Guid ProductId,
    string ProductName,
    int UnitsSold,
    decimal Revenue);

public record LowStockProductDto(
    Guid ProductId,
    string ProductName,
    string Sku,
    int QuantityAvailable,
    int LowStockThreshold);
