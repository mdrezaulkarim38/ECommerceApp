using System.Diagnostics;

namespace ECommerceApp.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        var statusCode = context.Response.StatusCode;
        var logLevel = statusCode >= StatusCodes.Status500InternalServerError
            ? LogLevel.Error
            : statusCode >= StatusCodes.Status400BadRequest
                ? LogLevel.Warning
                : LogLevel.Information;

        using var scope = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["traceId"] = context.TraceIdentifier,
            ["requestMethod"] = context.Request.Method,
            ["requestPath"] = context.Request.Path.Value,
            ["statusCode"] = statusCode,
            ["elapsedMilliseconds"] = stopwatch.ElapsedMilliseconds,
            ["remoteIpAddress"] = context.Connection.RemoteIpAddress?.ToString(),
            ["userId"] = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            ["userName"] = context.User.Identity?.Name
        });

        _logger.Log(
            logLevel,
            "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms",
            context.Request.Method,
            context.Request.Path.Value,
            statusCode,
            stopwatch.ElapsedMilliseconds);
    }
}
