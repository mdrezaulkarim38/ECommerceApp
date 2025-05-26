using ECommerceApp.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerceApp.Web.Pages;

public class IndexModel : PageModel
{
    private readonly HttpClient _http;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("API");
    }

    public List<Product> Products { get; set; } = new();

    public async Task OnGetAsync()
    {
        var result = await _http.GetFromJsonAsync<ProductResponse>("api/products?page=1&pageSize=8");
        Products = result?.Products ?? new List<Product>();
    }

    public class ProductResponse
    {
        public int Total { get; set; }
        public List<Product> Products { get; set; } = new();
    }
}