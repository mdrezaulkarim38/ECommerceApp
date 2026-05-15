namespace EcommerceAppApi.Application.DTOs;

public class DashboardDto
{
    public decimal Revenue { get; set; }
    public int TotalOrders { get; set; }
    public int TotalUsers { get; set; }
    public int TotalProducts { get; set; }
    public List<ProductDto> LowStockProducts { get; set; } = new();
    public List<ProductDto> TopProducts { get; set; } = new();
    public List<OrderDto> RecentOrders { get; set; } = new();
    public List<SalesDataPoint> SalesData { get; set; } = new();
}

public class SalesDataPoint
{
    public string Date { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Orders { get; set; }
}

public class UserListDto
{
    public List<UserProfileDto> Users { get; set; } = new();
    public int TotalUsers { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class SettingsDto
{
    public string StoreName { get; set; } = "SmartShop";
    public string Email { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public decimal TaxRate { get; set; } = 8;
    public bool RecommendationEnabled { get; set; } = true;
    public bool ForecastingEnabled { get; set; } = true;
    public string RetrainSchedule { get; set; } = "Weekly on Sunday";
}

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int StockQuantity { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public string? MainImageUrl { get; set; }
    public string? Sku { get; set; }
    public bool IsFeatured { get; set; }
    public string? Specs { get; set; }
    public string? Features { get; set; }
}

public class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int StockQuantity { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public string? MainImageUrl { get; set; }
    public string? Sku { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public string? Specs { get; set; }
    public string? Features { get; set; }
}
