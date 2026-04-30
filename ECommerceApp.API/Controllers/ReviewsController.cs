using ECommerceApp.API.Data;
using ECommerceApp.API.Dtos;
using ECommerceApp.API.Models;
using ECommerceApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.API.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly CurrentUserService _currentUser;

    public ReviewsController(AppDbContext db, CurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetProductReviews(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var reviews = await _db.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.ProductId == productId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(reviews.Select(ToDto).ToList());
    }

    [Authorize]
    [HttpPost("product/{productId:guid}")]
    public async Task<ActionResult<ReviewDto>> CreateOrUpdateReview(
        Guid productId,
        ReviewUpsertRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Rating is < 1 or > 5)
        {
            return BadRequest(new ApiMessage("Rating must be between 1 and 5."));
        }

        var productExists = await _db.Products.AnyAsync(p => p.Id == productId && p.IsActive, cancellationToken);
        if (!productExists)
        {
            return NotFound(new ApiMessage("Product was not found."));
        }

        var userId = _currentUser.UserId;
        var review = await _db.Reviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId, cancellationToken);

        if (review is null)
        {
            review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = request.Rating,
                Title = request.Title?.Trim(),
                Comment = request.Comment?.Trim()
            };
            _db.Reviews.Add(review);
        }
        else
        {
            review.Rating = request.Rating;
            review.Title = request.Title?.Trim();
            review.Comment = request.Comment?.Trim();
            review.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);

        review = await _db.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .FirstAsync(r => r.ProductId == productId && r.UserId == userId, cancellationToken);

        return Ok(ToDto(review));
    }

    [Authorize]
    [HttpDelete("product/{productId:guid}")]
    public async Task<IActionResult> DeleteReview(Guid productId, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var review = await _db.Reviews
            .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId, cancellationToken);

        if (review is null)
        {
            return NotFound(new ApiMessage("Review was not found."));
        }

        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new ApiMessage("Review deleted successfully."));
    }

    private static ReviewDto ToDto(Review review)
    {
        return new ReviewDto(
            review.Id,
            review.ProductId,
            review.User?.FullName ?? review.User?.Email ?? "Customer",
            review.Rating,
            review.Title,
            review.Comment,
            review.CreatedAt);
    }
}
