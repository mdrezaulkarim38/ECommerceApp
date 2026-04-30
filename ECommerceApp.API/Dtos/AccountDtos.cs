namespace ECommerceApp.API.Dtos;

public record AccountDto(
    Guid Id,
    string Email,
    string? UserName,
    string FullName,
    string? PhoneNumber,
    string? AddressLine1,
    string? City,
    string? Country,
    IReadOnlyList<string> Roles);

public record UpdateProfileRequest(
    string FullName,
    string? PhoneNumber,
    string? AddressLine1,
    string? City,
    string? Country);

public record AdminUserDto(
    Guid Id,
    string Email,
    string FullName,
    bool EmailConfirmed,
    DateTimeOffset CreatedAt,
    IReadOnlyList<string> Roles);

public record UpdateUserRolesRequest(IReadOnlyList<string> Roles);
