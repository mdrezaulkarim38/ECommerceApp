namespace EcommerceAppApi.Domain.Entities;

public class Settings
{
    public int Id { get; set; }
    public string StoreName { get; set; } = "SmartShop";
    public string Email { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public decimal TaxRate { get; set; } = 8;
    public bool RecommendationEnabled { get; set; } = true;
    public bool ForecastingEnabled { get; set; } = true;
    public string RetrainSchedule { get; set; } = "Weekly on Sunday";
}
