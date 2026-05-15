using System.Security.Claims;
using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAppApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    public ReviewsController(IReviewService reviewService) => _reviewService = reviewService;

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<ApiResponse<List<ReviewDto>>>> GetProductReviews(int productId)
    {
        var reviews = await _reviewService.GetProductReviewsAsync(productId);
        return Ok(ApiResponse<List<ReviewDto>>.Ok(reviews));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReviewDto>>> AddReview([FromBody] AddReviewRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var review = await _reviewService.AddReviewAsync(userId, request);
            return Ok(ApiResponse<ReviewDto>.Ok(review, "Review added"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ReviewDto>.Error(ex.Message));
        }
    }

    [HttpGet("product/{productId}/rating")]
    public async Task<ActionResult<ApiResponse<double>>> GetRating(int productId)
    {
        var rating = await _reviewService.GetAverageRatingAsync(productId);
        return Ok(ApiResponse<double>.Ok(rating));
    }
}
