using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAppApi.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly IAdminService _adminService;
    public DashboardController(IAdminService adminService) => _adminService = adminService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<DashboardDto>>> GetDashboard()
    {
        var dashboard = await _adminService.GetDashboardAsync();
        return Ok(ApiResponse<DashboardDto>.Ok(dashboard));
    }
}
