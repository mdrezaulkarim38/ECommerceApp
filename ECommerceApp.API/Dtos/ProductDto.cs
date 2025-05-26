namespace ECommerceApp.API.Dtos;
public class ProductDto
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPercent { get; set; }
    public DateTime? DiscountStartDate { get; set; }
    public DateTime? DiscountEndDate { get; set; }
    public IFormFile? ImageFile { get; set; }
}