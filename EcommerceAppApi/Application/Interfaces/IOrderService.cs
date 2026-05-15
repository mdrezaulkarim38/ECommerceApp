using EcommerceAppApi.Application.DTOs;

namespace EcommerceAppApi.Application.Interfaces;

public interface IOrderService
{
    Task<CheckoutQuoteResponse> GetQuoteAsync(CheckoutQuoteRequest request);
    Task<OrderDetailDto> PlaceOrderAsync(int userId, PlaceOrderRequest request);
    Task<List<OrderDto>> GetUserOrdersAsync(int userId);
    Task<OrderDetailDto?> GetOrderByIdAsync(int id, int userId);
    Task<OrderDetailDto?> GetOrderByNumberAsync(string orderNumber, int userId);
    Task<List<OrderDto>> GetAllOrdersAsync();
    Task<OrderDetailDto> UpdateOrderStatusAsync(int orderId, string status, string? note = null);
}
