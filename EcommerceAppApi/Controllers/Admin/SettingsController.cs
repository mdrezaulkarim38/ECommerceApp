using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAppApi.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class SettingsController : ControllerBase
{
    private readonly IAdminService _adminService;
    public SettingsController(IAdminService adminService) => _adminService = adminService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<SettingsDto>>> GetSettings()
    {
        var settings = await _adminService.GetSettingsAsync();
        return Ok(ApiResponse<SettingsDto>.Ok(settings));
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<string>>> UpdateSettings([FromBody] SettingsDto request)
    {
        await _adminService.UpdateSettingsAsync(request);
        return Ok(ApiResponse<string>.Ok("", "Settings updated"));
    }
}
