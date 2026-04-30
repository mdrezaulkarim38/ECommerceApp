namespace ECommerceApp.API.Dtos;

public record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    bool IsActive,
    int ProductCount);

public record CategoryUpsertRequest(
    string Name,
    string? Slug,
    string? Description,
    bool IsActive = true);
