namespace ECommerceApp.API.Services;

public interface IFileStorageService
{
    Task<string?> SaveProductImageAsync(IFormFile? imageFile, CancellationToken cancellationToken);
    Task DeleteAsync(string? relativeUrl);
}

public class FileStorageService : IFileStorageService
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private readonly IWebHostEnvironment _environment;

    public FileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string?> SaveProductImageAsync(IFormFile? imageFile, CancellationToken cancellationToken)
    {
        if (imageFile is null || imageFile.Length == 0)
        {
            return null;
        }

        if (imageFile.Length > 5 * 1024 * 1024)
        {
            throw new ArgumentException("Product image must be 5 MB or smaller.");
        }

        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Product image must be JPG, PNG, or WEBP.");
        }

        var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var uploadsPath = Path.Combine(webRoot, "uploads", "products");
        Directory.CreateDirectory(uploadsPath);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadsPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.CreateNew);
        await imageFile.CopyToAsync(stream, cancellationToken);

        return $"/uploads/products/{fileName}";
    }

    public Task DeleteAsync(string? relativeUrl)
    {
        if (string.IsNullOrWhiteSpace(relativeUrl))
        {
            return Task.CompletedTask;
        }

        var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var normalized = relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var filePath = Path.GetFullPath(Path.Combine(webRoot, normalized));
        var uploadsRoot = Path.GetFullPath(Path.Combine(webRoot, "uploads", "products"));

        if (filePath.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase) && File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
