using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Web.Models;

public class Product
{
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string? Name { get; set; }
    [Required]
    [StringLength(100)]
    public string? Slug { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPercent { get; set; }
    public DateTime? DiscountStartDate { get; set; }
    public DateTime? DiscountEndDate { get; set; }
    [StringLength(900)]
    public string? ImageUrl { get; set; }
}