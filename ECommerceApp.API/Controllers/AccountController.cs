using ECommerceApp.API.Data;
using ECommerceApp.API.Dtos;
using ECommerceApp.API.Models;
using ECommerceApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.API.Controllers;

[ApiController]
[Authorize]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly CurrentUserService _currentUser;

    public AccountController(
        AppDbContext db,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        CurrentUserService currentUser)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _currentUser = currentUser;
    }

    [HttpGet("me")]
    public async Task<ActionResult<AccountDto>> GetMe()
    {
        var user = await _userManager.FindByIdAsync(_currentUser.UserId.ToString());
        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(await ToAccountDtoAsync(user));
    }

    [HttpPut("me")]
    public async Task<ActionResult<AccountDto>> UpdateMe(UpdateProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(_currentUser.UserId.ToString());
        if (user is null)
        {
            return Unauthorized();
        }

        user.FullName = request.FullName.Trim();
        user.PhoneNumber = request.PhoneNumber?.Trim();
        user.AddressLine1 = request.AddressLine1?.Trim();
        user.City = request.City?.Trim();
        user.Country = request.Country?.Trim();

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(new ApiMessage(string.Join("; ", result.Errors.Select(e => e.Description))));
        }

        return Ok(await ToAccountDtoAsync(user));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/users")]
    public async Task<ActionResult<IReadOnlyList<AdminUserDto>>> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _db.Users
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .Take(200)
            .ToListAsync(cancellationToken);

        var result = new List<AdminUserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new AdminUserDto(
                user.Id,
                user.Email ?? string.Empty,
                user.FullName,
                user.EmailConfirmed,
                user.CreatedAt,
                roles.ToList()));
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("admin/users/{id:guid}/roles")]
    public async Task<IActionResult> UpdateUserRoles(Guid id, UpdateUserRolesRequest request)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound(new ApiMessage("User was not found."));
        }

        var requestedRoles = request.Roles
            .Select(r => r.Trim())
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var role in requestedRoles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                return BadRequest(new ApiMessage($"Role '{role}' does not exist."));
            }
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            return BadRequest(new ApiMessage(string.Join("; ", removeResult.Errors.Select(e => e.Description))));
        }

        var addResult = await _userManager.AddToRolesAsync(user, requestedRoles);
        if (!addResult.Succeeded)
        {
            return BadRequest(new ApiMessage(string.Join("; ", addResult.Errors.Select(e => e.Description))));
        }

        return Ok(new ApiMessage("User roles updated successfully."));
    }

    private async Task<AccountDto> ToAccountDtoAsync(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return new AccountDto(
            user.Id,
            user.Email ?? string.Empty,
            user.UserName,
            user.FullName,
            user.PhoneNumber,
            user.AddressLine1,
            user.City,
            user.Country,
            roles.ToList());
    }
}
