using System.Net;
using System.Text.Json;

namespace ECommerceApp.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        object response;
        switch (exception)
        {
            case UnauthorizedAccessException unauthorizedEx:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new
                {
                    context.Response.StatusCode,
                    Message = "Unauthorized access.",
                    Detailed = unauthorizedEx.Message
                };
                break;

            case ArgumentException argEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    context.Response.StatusCode,
                    Message = "Invalid request.",
                    Detailed = argEx.Message
                };
                break;

            case InvalidOperationException invOpEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    context.Response.StatusCode,
                    Message = "Invalid request.",
                    Detailed = invOpEx.Message
                };
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new
                {
                    context.Response.StatusCode,
                    Message = "Internal Server error.",
                    Detailed = context.RequestServices.GetService<IWebHostEnvironment>()!.IsDevelopment() ? exception.Message : string.Empty
                };
                break;
        }

        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);
        var result = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(result);
    }
}