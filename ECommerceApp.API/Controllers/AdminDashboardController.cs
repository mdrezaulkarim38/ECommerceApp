using ECommerceApp.API.Data;
using ECommerceApp.API.Dtos;
using ECommerceApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/dashboard")]
public class AdminDashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminDashboardController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(CancellationToken cancellationToken)
    {
        var since = DateTimeOffset.UtcNow.AddDays(-30);

        var totalProducts = await _db.Products.CountAsync(cancellationToken);
        var totalCategories = await _db.Categories.CountAsync(cancellationToken);
        var totalCustomers = await _db.Users.CountAsync(cancellationToken);
        var totalOrders = await _db.Orders.CountAsync(cancellationToken);
        var pendingOrders = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Pending, cancellationToken);
        var lowStockProducts = await _db.InventoryItems
            .CountAsync(i => i.Product!.IsActive && i.QuantityAvailable <= i.LowStockThreshold, cancellationToken);
        var revenueLast30Days = await _db.Orders
            .Where(o => o.CreatedAt >= since && o.Status != OrderStatus.Cancelled)
            .SumAsync(o => (decimal?)o.Total, cancellationToken) ?? 0;

        var topProductRows = await _db.OrderItems
            .AsNoTracking()
            .Where(i => i.Order!.Status != OrderStatus.Cancelled)
            .GroupBy(i => new { i.ProductId, i.ProductName })
            .Select(g => new
            {
                g.Key.ProductId,
                g.Key.ProductName,
                UnitsSold = g.Sum(i => i.Quantity),
                Revenue = g.Sum(i => i.LineTotal)
            })
            .OrderByDescending(i => i.UnitsSold)
            .Take(5)
            .ToListAsync(cancellationToken);

        var topProducts = topProductRows
            .Select(i => new TopProductDto(i.ProductId, i.ProductName, i.UnitsSold, i.Revenue))
            .ToList();

        var lowStockItems = await _db.InventoryItems
            .AsNoTracking()
            .Include(i => i.Product)
            .Where(i => i.Product!.IsActive && i.QuantityAvailable <= i.LowStockThreshold)
            .OrderBy(i => i.QuantityAvailable)
            .Take(10)
            .Select(i => new LowStockProductDto(
                i.ProductId,
                i.Product!.Name,
                i.Product.Sku,
                i.QuantityAvailable,
                i.LowStockThreshold))
            .ToListAsync(cancellationToken);

        return Ok(new DashboardSummaryDto(
            totalProducts,
            totalCategories,
            totalCustomers,
            totalOrders,
            revenueLast30Days,
            pendingOrders,
            lowStockProducts,
            topProducts,
            lowStockItems));
    }
}
