using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using EcommerceAppApi.Domain.Entities;
using EcommerceAppApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAppApi.Application.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;
    public ReviewService(ApplicationDbContext context) => _context = context;

    public async Task<List<ReviewDto>> GetProductReviewsAsync(int productId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                UserId = r.UserId,
                UserName = r.User.Name,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ReviewDto> AddReviewAsync(int userId, AddReviewRequest request)
    {
        var hasOrdered = await _context.Orders
            .AnyAsync(o => o.UserId == userId && o.Items.Any(oi => oi.ProductId == request.ProductId));

        var review = new Review
        {
            ProductId = request.ProductId,
            UserId = userId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        await UpdateProductRating(request.ProductId);

        var user = await _context.Users.FindAsync(userId);
        return new ReviewDto
        {
            Id = review.Id,
            ProductId = review.ProductId,
            UserId = review.UserId,
            UserName = user?.Name ?? "",
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }

    public async Task<double> GetAverageRatingAsync(int productId)
    {
        return await _context.Reviews
            .Where(r => r.ProductId == productId)
            .AverageAsync(r => (double?)r.Rating) ?? 0;
    }

    private async Task UpdateProductRating(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return;

        product.AverageRating = await _context.Reviews
            .Where(r => r.ProductId == productId)
            .AverageAsync(r => (double?)r.Rating) ?? 0;
        product.TotalReviews = await _context.Reviews
            .CountAsync(r => r.ProductId == productId);

        await _context.SaveChangesAsync();
    }
}
