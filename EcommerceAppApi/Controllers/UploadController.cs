using EcommerceAppApi.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace EcommerceAppApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UploadController> _logger;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
    private const int MaxDimension = 1200;
    private const int JpegQuality = 80;
    private const long MaxFileSize = 10 * 1024 * 1024;

    public UploadController(IWebHostEnvironment env, ILogger<UploadController> logger)
    {
        _env = env;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<string>>> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<string>.Error("No file uploaded"));

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return BadRequest(ApiResponse<string>.Error("Invalid file type. Allowed: jpg, jpeg, png, webp, gif"));

        if (file.Length > MaxFileSize)
            return BadRequest(ApiResponse<string>.Error("File too large. Maximum size: 10MB"));

        var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsPath))
            Directory.CreateDirectory(uploadsPath);

        var outputExt = extension == ".png" ? ".png" : ".jpg";
        var fileName = $"{Guid.NewGuid():N}{outputExt}";
        var filePath = Path.Combine(uploadsPath, fileName);

        try
        {
            using var image = await Image.LoadAsync(file.OpenReadStream());

            if (image.Width > MaxDimension || image.Height > MaxDimension)
            {
                var ratio = Math.Min((double)MaxDimension / image.Width, (double)MaxDimension / image.Height);
                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);
                image.Mutate(x => x.Resize(newWidth, newHeight));
            }

            SixLabors.ImageSharp.Formats.IImageEncoder encoder = outputExt == ".png"
                ? new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression }
                : new JpegEncoder { Quality = JpegQuality };

            await image.SaveAsync(filePath, encoder);

            var originalSize = file.Length;
            var compressedSize = new FileInfo(filePath).Length;
            var savings = (1 - (double)compressedSize / originalSize) * 100;

            var url = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
            _logger.LogInformation(
                "Image uploaded: {FileName} ({OriginalSize} -> {CompressedSize} bytes, saved {Savings:F1}%)",
                fileName, originalSize, compressedSize, savings);

            return Ok(ApiResponse<string>.Ok(url,
                $"Image uploaded and compressed ({savings:F0}% smaller)"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process image upload");
            return BadRequest(ApiResponse<string>.Error("Failed to process image"));
        }
    }

    [HttpDelete("{fileName}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<ApiResponse<string>> DeleteImage(string fileName)
    {
        var safeName = Path.GetFileName(fileName);
        var filePath = Path.Combine(_env.WebRootPath, "uploads", safeName);

        if (!System.IO.File.Exists(filePath))
            return NotFound(ApiResponse<string>.Error("File not found"));

        System.IO.File.Delete(filePath);
        _logger.LogInformation("Image deleted: {FileName}", safeName);
        return Ok(ApiResponse<string>.Ok("", "Image deleted"));
    }
}
