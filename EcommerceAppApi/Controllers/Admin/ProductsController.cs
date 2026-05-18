using System.Text.RegularExpressions;
using ClosedXML.Excel;
using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using EcommerceAppApi.Domain.Entities;
using EcommerceAppApi.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAppApi.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ApplicationDbContext _context;
    public ProductsController(IProductService productService, ApplicationDbContext context)
    {
        _productService = productService;
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductRequest request)
    {
        var product = await _productService.CreateProductAsync(request);
        return Ok(ApiResponse<ProductDto>.Ok(product, "Product created"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            var product = await _productService.UpdateProductAsync(id, request);
            return Ok(ApiResponse<ProductDto>.Ok(product, "Product updated"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ProductDto>.Error(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        if (!result) return NotFound(ApiResponse<string>.Error("Product not found"));
        return Ok(ApiResponse<string>.Ok("", "Product deleted"));
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .OrderBy(p => p.Id)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Products");

        var headers = new[] { "Name", "Description", "Price", "CompareAtPrice", "StockQuantity", "Category", "Brand", "SKU", "MainImageUrl" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
        }

        int row = 2;
        foreach (var p in products)
        {
            ws.Cell(row, 1).Value = p.Name;
            ws.Cell(row, 2).Value = p.Description;
            ws.Cell(row, 3).Value = (double)p.Price;
            ws.Cell(row, 4).Value = p.CompareAtPrice.HasValue ? (double)p.CompareAtPrice.Value : 0;
            ws.Cell(row, 5).Value = p.StockQuantity;
            ws.Cell(row, 6).Value = p.Category?.Name ?? "";
            ws.Cell(row, 7).Value = p.Brand?.Name ?? "";
            ws.Cell(row, 8).Value = p.Sku ?? "";
            ws.Cell(row, 9).Value = p.MainImageUrl ?? "";
            row++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "products.xlsx");
    }

    [HttpGet("template")]
    public IActionResult Template()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Template");

        var headers = new[] { "Name", "Description", "Price", "CompareAtPrice", "StockQuantity", "Category", "Brand", "SKU", "MainImageUrl" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
        }

        ws.Cell(2, 1).Value = "Example Product";
        ws.Cell(2, 3).Value = 19.99;
        ws.Cell(2, 5).Value = 100;

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "product-import-template.xlsx");
    }

    [HttpPost("import")]
    public async Task<ActionResult<ApiResponse<object>>> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<object>.Error("No file uploaded"));

        var categories = await _context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);
        var brands = await _context.Brands.ToDictionaryAsync(b => b.Name, b => b.Id);

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet(1);
        var rows = ws.RangeUsed().RowsUsed().Skip(1);

        var products = new List<Product>();
        var errors = new List<string>();
        int rowNum = 1;

        foreach (var row in rows)
        {
            rowNum++;
            try
            {
                var name = row.Cell(1).GetString().Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    errors.Add($"Row {rowNum}: Name is required");
                    continue;
                }

                var categoryName = row.Cell(6).GetString().Trim();
                int? categoryId = null;
                if (!string.IsNullOrWhiteSpace(categoryName))
                {
                    if (categories.TryGetValue(categoryName, out var cid))
                        categoryId = cid;
                    else
                        errors.Add($"Row {rowNum}: Category '{categoryName}' not found");
                }

                var brandName = row.Cell(7).GetString().Trim();
                int? brandId = null;
                if (!string.IsNullOrWhiteSpace(brandName))
                {
                    if (brands.TryGetValue(brandName, out var bid))
                        brandId = bid;
                    else
                        errors.Add($"Row {rowNum}: Brand '{brandName}' not found");
                }

                var product = new Product
                {
                    Name = name,
                    Slug = Regex.Replace(name.ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-'),
                    Description = row.Cell(2).GetString().Trim(),
                    Price = ParseDecimal(row.Cell(3)) ?? 0,
                    CompareAtPrice = ParseDecimal(row.Cell(4)),
                    StockQuantity = (int)(ParseDecimal(row.Cell(5)) ?? 0),
                    CategoryId = categoryId,
                    BrandId = brandId,
                    Sku = row.Cell(8).GetString().Trim(),
                    MainImageUrl = row.Cell(9).GetString().Trim(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                products.Add(product);
            }
            catch (Exception ex)
            {
                errors.Add($"Row {rowNum}: {ex.Message}");
            }
        }

        if (products.Count > 0)
        {
            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();
        }

        return Ok(ApiResponse<object>.Ok(new
        {
            imported = products.Count,
            errors = errors
        }, $"{products.Count} product(s) imported"));
    }

    private static decimal? ParseDecimal(IXLCell cell)
    {
        if (cell.IsEmpty()) return null;
        var val = cell.GetString().Trim();
        if (string.IsNullOrWhiteSpace(val)) return null;
        if (decimal.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var result))
            return result;
        return null;
    }
}
