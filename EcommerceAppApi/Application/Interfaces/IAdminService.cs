using EcommerceAppApi.Application.DTOs;

namespace EcommerceAppApi.Application.Interfaces;

public interface IAdminService
{
    Task<DashboardDto> GetDashboardAsync();
    Task<List<OrderDto>> GetAllOrdersAsync();
    Task<UserListDto> GetUsersAsync(int page = 1, int pageSize = 20, string? search = null);
    Task<bool> ToggleUserBlockAsync(int userId);
    Task<bool> ToggleUserRoleAsync(int userId);
    Task<bool> DeleteUserAsync(int userId);
    Task<SettingsDto> GetSettingsAsync();
    Task UpdateSettingsAsync(SettingsDto settings);
}
