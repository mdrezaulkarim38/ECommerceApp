using EcommerceAppApi.Application.DTOs;

namespace EcommerceAppApi.Application.Interfaces;

public interface IReviewService
{
    Task<List<ReviewDto>> GetProductReviewsAsync(int productId);
    Task<ReviewDto> AddReviewAsync(int userId, AddReviewRequest request);
    Task<double> GetAverageRatingAsync(int productId);
}
