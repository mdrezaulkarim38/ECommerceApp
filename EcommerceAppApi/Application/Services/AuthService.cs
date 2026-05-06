using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EcommerceAppApi.Application.Interfaces;
using EcommerceAppApi.Domain.Entities;
using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Domain.Enums;
using EcommerceAppApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceAppApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // TODO: Implement login logic
        throw new NotImplementedException();
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            throw new Exception("Passwords do not match");
        }
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new Exception("Email already exists");
        }
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Role.User,
            CreatedAt = DateTime.UtcNow,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        _context.Carts.Add(new Cart { UserId = user.Id, CreatedAt = DateTime.UtcNow });
        _context.Wishlists.Add(new Wishlist { UserId = user.Id, CreatedAt = DateTime.UtcNow });

        if (!string.IsNullOrEmpty(request.Address))
        {
            _context.Addresses.Add(new Address
            {
                UserId = user.Id,
                FullName = request.Name,
                Street = request.Address,
                City = "",
                State = "",
                ZipCode = "",
                Country = "",
                PhoneNumber = request.PhoneNumber,
                IsDefault = true
            });
        }
        await _context.SaveChangesAsync();
        return await GenerateAuthResponse(user);
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
    private async Task<AuthResponse> GenerateAuthResponse(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        _context.UserRefreshTokens.Add(new UserRefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = new UserProfileDto
            {
              Id = user.Id,
              Email = user.Email,
              Name = user.Name,
              PhoneNumber = user.PhoneNumber,
              Role = user.Role.ToString(),
            },
            Role = user.Role.ToString()
        };
    }

    private string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "K5gR2ASbU4oIupFbU+LnfPW9Xrl/lzuQVFAs1xCtqee0Ljk4t662BDoQzwr7eJMO16nYAy2BAQXzOgYbVo/wgg=="));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var token = new JwtSecurityToken(issuer: _configuration["Jwt:Issuer"],audience:_configuration["Jwt:Audience"],claims: claims, expires: DateTime.UtcNow.AddHours(2), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
