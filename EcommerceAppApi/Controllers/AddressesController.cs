using System.Security.Claims;
using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAppApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;
    public AddressesController(IAddressService addressService) => _addressService = addressService;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<AddressDto>>>> GetAddresses()
    {
        var addresses = await _addressService.GetUserAddressesAsync(UserId);
        return Ok(ApiResponse<List<AddressDto>>.Ok(addresses));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AddressDto>>> AddAddress([FromBody] AddressDto request)
    {
        var address = await _addressService.AddAddressAsync(UserId, request);
        return Ok(ApiResponse<AddressDto>.Ok(address, "Address added"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<AddressDto>>> UpdateAddress(int id, [FromBody] AddressDto request)
    {
        try
        {
            var address = await _addressService.UpdateAddressAsync(UserId, id, request);
            return Ok(ApiResponse<AddressDto>.Ok(address, "Address updated"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<AddressDto>.Error(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteAddress(int id)
    {
        var result = await _addressService.DeleteAddressAsync(UserId, id);
        if (!result) return NotFound(ApiResponse<string>.Error("Address not found"));
        return Ok(ApiResponse<string>.Ok("", "Address deleted"));
    }

    [HttpPut("{id}/default")]
    public async Task<ActionResult<ApiResponse<string>>> SetDefault(int id)
    {
        await _addressService.SetDefaultAddressAsync(UserId, id);
        return Ok(ApiResponse<string>.Ok("", "Default address updated"));
    }
}
