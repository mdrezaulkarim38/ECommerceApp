using EcommerceAppApi.Application.Interfaces;
using EcommerceAppApi.Domain.Entities;
using EcommerceAppApi.Application.DTOs;
namespace EcommerceAppApi.Application.Services;

public class AuthService : IAuthService
{
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // TODO: Implement login logic
        throw new NotImplementedException();
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // TODO: Implement registration logic
        throw new NotImplementedException();
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        // TODO: Implement refresh token logic
        throw new NotImplementedException();
    }

    public async Task LogoutAsync(string refreshToken)
    {
        // TODO: Implement logout logic
        throw new NotImplementedException();
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        // TODO: Implement forgot password logic
        throw new NotImplementedException();
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
    {
        // TODO: Implement reset password logic
        throw new NotImplementedException();
    }

    public async Task<UserProfileDto> GetCurrentUserAsync(int userId)
    {
        // TODO: Implement get current user logic
        throw new NotImplementedException();
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        // TODO: Implement change password logic
        throw new NotImplementedException();
    }
}
