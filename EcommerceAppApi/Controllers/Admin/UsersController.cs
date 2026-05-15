using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAppApi.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IAdminService _adminService;
    public UsersController(IAdminService adminService) => _adminService = adminService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<UserListDto>>> GetUsers(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
    {
        var users = await _adminService.GetUsersAsync(page, pageSize, search);
        return Ok(ApiResponse<UserListDto>.Ok(users));
    }

    [HttpPut("{id}/toggle-block")]
    public async Task<ActionResult<ApiResponse<string>>> ToggleBlock(int id)
    {
        var result = await _adminService.ToggleUserBlockAsync(id);
        if (!result) return NotFound(ApiResponse<string>.Error("User not found"));
        return Ok(ApiResponse<string>.Ok("", "User block status toggled"));
    }

    [HttpPut("{id}/toggle-role")]
    public async Task<ActionResult<ApiResponse<string>>> ToggleRole(int id)
    {
        var result = await _adminService.ToggleUserRoleAsync(id);
        if (!result) return NotFound(ApiResponse<string>.Error("User not found"));
        return Ok(ApiResponse<string>.Ok("", "User role toggled"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var result = await _adminService.DeleteUserAsync(id);
        if (!result) return NotFound(ApiResponse<string>.Error("User not found"));
        return Ok(ApiResponse<string>.Ok("", "User deleted"));
    }
}
