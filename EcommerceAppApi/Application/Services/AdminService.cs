using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using EcommerceAppApi.Domain.Enums;
using EcommerceAppApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAppApi.Application.Services;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;
    private readonly IOrderService _orderService;
    public AdminService(ApplicationDbContext context, IOrderService orderService)
    {
        _context = context;
        _orderService = orderService;
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var now = DateTime.UtcNow;
        var totalRevenue = await _context.Orders
            .Where(o => o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.Refunded)
            .SumAsync(o => o.TotalAmount);

        var totalOrders = await _context.Orders.CountAsync();
        var totalUsers = await _context.Users.CountAsync(u => u.Role == Role.User);
        var totalProducts = await _context.Products.CountAsync(p => p.IsActive);

        var lowStockProducts = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.IsActive && p.StockQuantity <= 5)
            .OrderBy(p => p.StockQuantity)
            .Take(10)
            .Select(p => MapToProductDto(p))
            .ToListAsync();

        var topProducts = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.TotalReviews)
            .ThenByDescending(p => p.AverageRating)
            .Take(10)
            .Select(p => MapToProductDto(p))
            .ToListAsync();

        var recentOrders = await _context.Orders
            .Include(o => o.User)
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.User.Name,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                PaymentMethod = o.PaymentMethod,
                CreatedAt = o.CreatedAt,
                ItemCount = o.Items.Count
            })
            .ToListAsync();

        var salesData = await _context.Orders
            .Where(o => o.CreatedAt >= now.AddDays(-30))
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new SalesDataPoint
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                Revenue = g.Sum(o => o.TotalAmount),
                Orders = g.Count()
            })
            .OrderBy(s => s.Date)
            .ToListAsync();

        return new DashboardDto
        {
            Revenue = totalRevenue,
            TotalOrders = totalOrders,
            TotalUsers = totalUsers,
            TotalProducts = totalProducts,
            LowStockProducts = lowStockProducts,
            TopProducts = topProducts,
            RecentOrders = recentOrders,
            SalesData = salesData
        };
    }

    public Task<List<OrderDto>> GetAllOrdersAsync()
    {
        return _orderService.GetAllOrdersAsync();
    }

    public async Task<UserListDto> GetUsersAsync(int page = 1, int pageSize = 20, string? search = null)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.Name.Contains(search) || u.Email.Contains(search));

        var totalUsers = await query.CountAsync();

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserProfileDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role.ToString()
            })
            .ToListAsync();

        return new UserListDto
        {
            Users = users,
            TotalUsers = totalUsers,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<bool> ToggleUserBlockAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.Blocked = !user.Blocked;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleUserRoleAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.Role = user.Role == Role.Admin ? Role.User : Role.Admin;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<SettingsDto> GetSettingsAsync()
    {
        var settings = await _context.Settings.FindAsync(1);
        if (settings == null) return new SettingsDto();
        return new SettingsDto
        {
            StoreName = settings.StoreName,
            Email = settings.Email,
            Currency = settings.Currency,
            TaxRate = settings.TaxRate,
            RecommendationEnabled = settings.RecommendationEnabled,
            ForecastingEnabled = settings.ForecastingEnabled,
            RetrainSchedule = settings.RetrainSchedule
        };
    }

    public async Task UpdateSettingsAsync(SettingsDto request)
    {
        var settings = await _context.Settings.FindAsync(1);
        if (settings == null)
        {
            _context.Settings.Add(new Domain.Entities.Settings
            {
                StoreName = request.StoreName,
                Email = request.Email,
                Currency = request.Currency,
                TaxRate = request.TaxRate,
                RecommendationEnabled = request.RecommendationEnabled,
                ForecastingEnabled = request.ForecastingEnabled,
                RetrainSchedule = request.RetrainSchedule
            });
        }
        else
        {
            settings.StoreName = request.StoreName;
            settings.Email = request.Email;
            settings.Currency = request.Currency;
            settings.TaxRate = request.TaxRate;
            settings.RecommendationEnabled = request.RecommendationEnabled;
            settings.ForecastingEnabled = request.ForecastingEnabled;
            settings.RetrainSchedule = request.RetrainSchedule;
        }
        await _context.SaveChangesAsync();
    }

    private static ProductDto MapToProductDto(Domain.Entities.Product p) => new()
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
        IsActive = p.IsActive
    };
}
