using EcommerceAppApi.Application.DTOs;

namespace EcommerceAppApi.Application.Interfaces;
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(string refressToken);
    Task<UserProfileDto> GetCurrentUserAsync(int userId);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    Task LogoutAsync(string refreshToken);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
}