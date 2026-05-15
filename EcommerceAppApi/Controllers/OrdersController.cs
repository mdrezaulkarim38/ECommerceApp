using System.Security.Claims;
using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAppApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrdersController(IOrderService orderService) => _orderService = orderService;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("quote")]
    public async Task<ActionResult<ApiResponse<CheckoutQuoteResponse>>> GetQuote([FromBody] CheckoutQuoteRequest request)
    {
        var quote = await _orderService.GetQuoteAsync(request);
        return Ok(ApiResponse<CheckoutQuoteResponse>.Ok(quote));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderDetailDto>>> PlaceOrder([FromBody] PlaceOrderRequest request)
    {
        try
        {
            var order = await _orderService.PlaceOrderAsync(UserId, request);
            return Ok(ApiResponse<OrderDetailDto>.Ok(order, "Order placed successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<OrderDetailDto>.Error(ex.Message));
        }
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetOrders()
    {
        var orders = await _orderService.GetUserOrdersAsync(UserId);
        return Ok(ApiResponse<List<OrderDto>>.Ok(orders));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderDetailDto>>> GetOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id, UserId);
        if (order == null) return NotFound(ApiResponse<OrderDetailDto>.Error("Order not found"));
        return Ok(ApiResponse<OrderDetailDto>.Ok(order));
    }

    [HttpGet("track/{orderNumber}")]
    public async Task<ActionResult<ApiResponse<OrderDetailDto>>> TrackOrder(string orderNumber)
    {
        var order = await _orderService.GetOrderByNumberAsync(orderNumber, UserId);
        if (order == null) return NotFound(ApiResponse<OrderDetailDto>.Error("Order not found"));
        return Ok(ApiResponse<OrderDetailDto>.Ok(order));
    }
}
