using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAppApi.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrdersController(IOrderService orderService) => _orderService = orderService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetAll()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(ApiResponse<List<OrderDto>>.Ok(orders));
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApiResponse<OrderDetailDto>>> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            var order = await _orderService.UpdateOrderStatusAsync(id, request.Status, request.Note);
            return Ok(ApiResponse<OrderDetailDto>.Ok(order, $"Order status updated to {request.Status}"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<OrderDetailDto>.Error(ex.Message));
        }
    }
}

public class UpdateOrderStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
}
