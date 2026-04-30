using ECommerceApp.API.Data;
using ECommerceApp.API.Middleware;
using ECommerceApp.API.Models;
using ECommerceApp.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole(options =>
{
    options.IncludeScopes = true;
    options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    options.UseUtcTimestamp = true;
    options.JsonWriterOptions = new JsonWriterOptions
    {
        Indented = builder.Environment.IsDevelopment()
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ECommerceApp API",
        Version = "v1",
        Description = "ASP.NET Core 10 backend for the AI-powered ecommerce platform."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "Identity bearer token",
        In = ParameterLocation.Header,
        Description = "Paste the access token returned by /api/auth/login."
    });

});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityApiEndpoints<AppUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactClient", policy =>
    {
        var origins = builder.Configuration
            .GetSection("AllowedOrigins")
            .Get<string[]>() ?? ["http://localhost:5173"];

        policy.SetIsOriginAllowed(origin =>
            origins.Contains(origin, StringComparer.OrdinalIgnoreCase) ||
            (builder.Environment.IsDevelopment() &&
             Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
             (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
              uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase))))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

var app = builder.Build();

if (app.Configuration.GetValue("Database:ApplyMigrationsOnStartup", true))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

app.UseCors("ReactClient");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapGroup("/api/auth")
    .WithTags("Auth")
    .MapIdentityApi<AppUser>();

app.MapControllers();
app.Run();
