using ECommerceApp.API.Data;
using ECommerceApp.API.Dtos;
using ECommerceApp.API.Extensions;
using ECommerceApp.API.Models;
using ECommerceApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.API.Controllers;

[ApiController]
[Authorize]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly CurrentUserService _currentUser;

    public OrdersController(AppDbContext db, CurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<OrderDto>> Checkout(
        CheckoutRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ShippingName) ||
            string.IsNullOrWhiteSpace(request.ShippingPhone) ||
            string.IsNullOrWhiteSpace(request.ShippingAddressLine1) ||
            string.IsNullOrWhiteSpace(request.ShippingCity) ||
            string.IsNullOrWhiteSpace(request.ShippingCountry))
        {
            return BadRequest(new ApiMessage("Shipping information is required."));
        }

        var userId = _currentUser.UserId;
        var cart = await _db.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p!.Inventory)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null || cart.Items.Count == 0)
        {
            return BadRequest(new ApiMessage("Cart is empty."));
        }

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var order = new Order
        {
            UserId = userId,
            OrderNumber = GenerateOrderNumber(),
            PaymentMethod = request.PaymentMethod,
            PaymentStatus = PaymentStatus.Pending,
            Status = OrderStatus.Pending,
            ShippingName = request.ShippingName.Trim(),
            ShippingPhone = request.ShippingPhone.Trim(),
            ShippingAddressLine1 = request.ShippingAddressLine1.Trim(),
            ShippingCity = request.ShippingCity.Trim(),
            ShippingCountry = request.ShippingCountry.Trim(),
            Notes = request.Notes?.Trim()
        };

        foreach (var cartItem in cart.Items)
        {
            var product = cartItem.Product;
            if (product is null || !product.IsActive)
            {
                return BadRequest(new ApiMessage("Cart contains an unavailable product."));
            }

            var inventory = product.Inventory;
            if (inventory is null || inventory.QuantityAvailable < cartItem.Quantity)
            {
                return BadRequest(new ApiMessage($"{product.Name} does not have enough stock."));
            }

            var unitPrice = product.GetEffectivePrice(now);
            var discount = Math.Max(0, product.Price - unitPrice);
            var lineTotal = unitPrice * cartItem.Quantity;

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Sku = product.Sku,
                UnitPrice = unitPrice,
                DiscountAmount = discount * cartItem.Quantity,
                Quantity = cartItem.Quantity,
                LineTotal = lineTotal
            });

            inventory.QuantityAvailable -= cartItem.Quantity;
        }

        order.Subtotal = order.Items.Sum(i => i.UnitPrice * i.Quantity);
        order.DiscountTotal = order.Items.Sum(i => i.DiscountAmount);
        order.ShippingTotal = order.Subtotal >= 5000 ? 0 : 100;
        order.TaxTotal = 0;
        order.Total = order.Subtotal + order.ShippingTotal + order.TaxTotal;

        _db.Orders.Add(order);
        _db.CartItems.RemoveRange(cart.Items);
        cart.UpdatedAt = now;

        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var created = await LoadOrderQuery()
            .FirstAsync(o => o.Id == order.Id, cancellationToken);

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, created.ToDto());
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetMyOrders(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var orders = await LoadOrderQuery()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(orders.Select(o => o.ToDto()).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var order = await LoadOrderQuery()
            .FirstOrDefaultAsync(o => o.Id == id && (o.UserId == userId || _currentUser.IsAdmin), cancellationToken);

        if (order is null)
        {
            return NotFound(new ApiMessage("Order was not found."));
        }

        return Ok(order.ToDto());
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetOrdersForAdmin(
        [FromQuery] OrderStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = LoadOrderQuery();
        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Ok(new PagedResult<OrderDto>(
            orders.Select(o => o.ToDto()).ToList(),
            page,
            pageSize,
            total));
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(
        Guid id,
        UpdateOrderStatusRequest request,
        CancellationToken cancellationToken)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (order is null)
        {
            return NotFound(new ApiMessage("Order was not found."));
        }

        order.Status = request.Status;
        if (request.PaymentStatus.HasValue)
        {
            order.PaymentStatus = request.PaymentStatus.Value;
        }
        order.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        var updated = await LoadOrderQuery()
            .FirstAsync(o => o.Id == id, cancellationToken);

        return Ok(updated.ToDto());
    }

    private IQueryable<Order> LoadOrderQuery()
    {
        return _db.Orders
            .AsNoTracking()
            .Include(o => o.Items);
    }

    private static string GenerateOrderNumber()
    {
        return $"EC-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
    }
}
