using ECommerceApp.API.Data;
using ECommerceApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/inventory")]
public class InventoryController : ControllerBase
{
    private readonly AppDbContext _db;

    public InventoryController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IReadOnlyList<LowStockProductDto>>> GetLowStock(CancellationToken cancellationToken)
    {
        var items = await _db.InventoryItems
            .AsNoTracking()
            .Include(i => i.Product)
            .Where(i => i.Product!.IsActive && i.QuantityAvailable <= i.LowStockThreshold)
            .OrderBy(i => i.QuantityAvailable)
            .Select(i => new LowStockProductDto(
                i.ProductId,
                i.Product!.Name,
                i.Product.Sku,
                i.QuantityAvailable,
                i.LowStockThreshold))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpPut("products/{productId:guid}")]
    public async Task<ActionResult<InventoryDto>> UpdateInventory(
        Guid productId,
        InventoryUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var inventory = await _db.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);

        if (inventory is null)
        {
            return NotFound(new ApiMessage("Inventory record was not found."));
        }

        inventory.QuantityAvailable = request.QuantityAvailable;
        inventory.LowStockThreshold = request.LowStockThreshold;
        inventory.LastRestockedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new InventoryDto(
            inventory.ProductId,
            inventory.QuantityAvailable,
            inventory.QuantityReserved,
            inventory.LowStockThreshold,
            inventory.QuantityAvailable <= inventory.LowStockThreshold,
            inventory.LastRestockedAt));
    }
}
