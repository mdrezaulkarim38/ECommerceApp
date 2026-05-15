using System.Text.Json;
using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using EcommerceAppApi.Domain.Entities;
using EcommerceAppApi.Domain.Enums;
using EcommerceAppApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAppApi.Application.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    public OrderService(ApplicationDbContext context) => _context = context;

    public async Task<CheckoutQuoteResponse> GetQuoteAsync(CheckoutQuoteRequest request)
    {
        var subtotal = request.CartItems.Sum(ci => ci.UnitPrice * ci.Quantity);
        var shippingCost = subtotal >= 200 || subtotal == 0 ? 0 : 8.50m;
        var discountAmount = 0m;
        var taxRate = 0.08m;

        if (!string.IsNullOrWhiteSpace(request.CouponCode) &&
            request.CouponCode.Trim().Equals("SMART10", StringComparison.OrdinalIgnoreCase))
        {
            discountAmount = subtotal * 0.10m;
        }

        var taxAmount = (subtotal - discountAmount) * taxRate;
        var totalAmount = subtotal + shippingCost + taxAmount - discountAmount;

        return new CheckoutQuoteResponse
        {
            Subtotal = subtotal,
            ShippingCost = shippingCost,
            TaxAmount = taxAmount,
            DiscountAmount = discountAmount,
            TotalAmount = totalAmount,
            AppliedCouponCode = discountAmount > 0 ? request.CouponCode?.Trim() : null
        };
    }

    public async Task<OrderDetailDto> PlaceOrderAsync(int userId, PlaceOrderRequest request)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || cart.Items.Count == 0)
            throw new InvalidOperationException("Cart is empty");

        Address? address = null;
        if (request.AddressId.HasValue)
        {
            address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == userId);
        }
        else if (request.ShippingAddress != null)
        {
            address = new Address
            {
                UserId = userId,
                FullName = request.ShippingAddress.FullName,
                Street = request.ShippingAddress.Street,
                City = request.ShippingAddress.City,
                State = request.ShippingAddress.State,
                ZipCode = request.ShippingAddress.ZipCode,
                Country = request.ShippingAddress.Country,
                PhoneNumber = request.ShippingAddress.PhoneNumber,
                IsDefault = false
            };
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
        }

        if (address == null)
            throw new InvalidOperationException("Shipping address is required");

        var subtotal = cart.Items.Sum(ci => ci.Product.Price * ci.Quantity);
        var shippingCost = subtotal >= 200 ? 0 : 8.50m;
        var discountAmount = 0m;
        var taxRate = 0.08m;

        if (!string.IsNullOrWhiteSpace(request.CouponCode) &&
            request.CouponCode.Trim().Equals("SMART10", StringComparison.OrdinalIgnoreCase))
        {
            discountAmount = subtotal * 0.10m;
        }

        var taxAmount = (subtotal - discountAmount) * taxRate;
        var totalAmount = subtotal + shippingCost + taxAmount - discountAmount;

        var orderNumber = $"ORD-{Random.Shared.Next(10000, 99999)}";
        while (await _context.Orders.AnyAsync(o => o.OrderNumber == orderNumber))
            orderNumber = $"ORD-{Random.Shared.Next(10000, 99999)}";

        var order = new Order
        {
            OrderNumber = orderNumber,
            UserId = userId,
            Subtotal = subtotal,
            ShippingCost = shippingCost,
            TaxAmount = taxAmount,
            DiscountAmount = discountAmount,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            CouponCode = discountAmount > 0 ? request.CouponCode?.Trim() : null,
            PaymentMethod = request.PaymentMethod,
            ShippingAddressJson = JsonSerializer.Serialize(new AddressDto
            {
                Id = address.Id,
                FullName = address.FullName,
                Street = address.Street,
                City = address.City,
                State = address.State,
                ZipCode = address.ZipCode,
                Country = address.Country,
                PhoneNumber = address.PhoneNumber,
                IsDefault = address.IsDefault
            }),
            CreatedAt = DateTime.UtcNow,
        };

        foreach (var cartItem in cart.Items)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = cartItem.ProductId,
                ProductName = cartItem.Product.Name,
                ProductImageUrl = cartItem.Product.MainImageUrl,
                UnitPrice = cartItem.Product.Price,
                Quantity = cartItem.Quantity,
                TotalPrice = cartItem.Product.Price * cartItem.Quantity
            });

            cartItem.Product.StockQuantity -= cartItem.Quantity;
        }

        order.StatusHistory.Add(new OrderStatusHistory
        {
            Status = OrderStatus.Pending,
            Note = "Order placed",
            ChangedAt = DateTime.UtcNow
        });

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cart.Items);
        await _context.SaveChangesAsync();

        return await MapToDetailDto(order);
    }

    public async Task<List<OrderDto>> GetUserOrdersAsync(int userId)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
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
    }

    public async Task<OrderDetailDto?> GetOrderByIdAsync(int id, int userId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory.OrderBy(sh => sh.ChangedAt))
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        return order == null ? null : await MapToDetailDto(order);
    }

    public async Task<OrderDetailDto?> GetOrderByNumberAsync(string orderNumber, int userId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory.OrderBy(sh => sh.ChangedAt))
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber && o.UserId == userId);

        return order == null ? null : await MapToDetailDto(order);
    }

    public async Task<List<OrderDto>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.User)
            .OrderByDescending(o => o.CreatedAt)
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
    }

    public async Task<OrderDetailDto> UpdateOrderStatusAsync(int orderId, string status, string? note = null)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) throw new KeyNotFoundException("Order not found");

        if (Enum.TryParse<OrderStatus>(status, true, out var newStatus))
        {
            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            if (newStatus == OrderStatus.Shipped) order.ShippedAt = DateTime.UtcNow;
            if (newStatus == OrderStatus.Delivered) order.DeliveredAt = DateTime.UtcNow;

            order.StatusHistory.Add(new OrderStatusHistory
            {
                Status = newStatus,
                Note = note,
                ChangedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        return await MapToDetailDto(order);
    }

    private async Task<OrderDetailDto> MapToDetailDto(Order order)
    {
        var address = string.IsNullOrEmpty(order.ShippingAddressJson)
            ? new AddressDto()
            : JsonSerializer.Deserialize<AddressDto>(order.ShippingAddressJson) ?? new AddressDto();

        return new OrderDetailDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Subtotal = order.Subtotal,
            ShippingCost = order.ShippingCost,
            TaxAmount = order.TaxAmount,
            DiscountAmount = order.DiscountAmount,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            PaymentMethod = order.PaymentMethod,
            TrackingNumber = order.TrackingNumber,
            TrackingUrl = order.TrackingUrl,
            CreatedAt = order.CreatedAt,
            ItemCount = order.Items.Count,
            ShippingAddress = address,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImageUrl = i.ProductImageUrl,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                TotalPrice = i.TotalPrice
            }).ToList(),
            StatusHistory = order.StatusHistory.Select(sh => new OrderStatusHistoryDto
            {
                Status = sh.Status.ToString(),
                Note = sh.Note,
                ChangedAt = sh.ChangedAt
            }).ToList()
        };
    }
}
