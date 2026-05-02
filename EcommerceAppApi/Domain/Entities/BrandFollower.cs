namespace EcommerceAppApi.Domain.Entities;

public class BrandFollower
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BrandId { get; set; }
    public DateTime FollowedAt { get; set; }
    public User User { get; set; } = null!;
    public Brand Brand { get; set; } = null!;
}