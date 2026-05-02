namespace EcommerceAppApi.Domain.Entities;

public class Brand
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<BrandFollower> Followers { get; set; } = new List<BrandFollower>();
}