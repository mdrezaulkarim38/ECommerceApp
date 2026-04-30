using System.Security.Claims;

namespace ECommerceApp.API.Services;

public class CurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userId, out var id))
            {
                return id;
            }

            throw new UnauthorizedAccessException("Authenticated user id is missing.");
        }
    }

    public bool IsAdmin => _httpContextAccessor.HttpContext?.User.IsInRole("Admin") == true;
}
