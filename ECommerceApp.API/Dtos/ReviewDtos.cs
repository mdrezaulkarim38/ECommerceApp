namespace ECommerceApp.API.Dtos;

public record ReviewUpsertRequest(int Rating, string? Title, string? Comment);

public record ReviewDto(
    Guid Id,
    Guid ProductId,
    string CustomerName,
    int Rating,
    string? Title,
    string? Comment,
    DateTimeOffset CreatedAt);
