using RateLimiter.CustomMiddlewares;
using RateLimiter.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var redisConnectionString =
    builder.Configuration["Redis:ConnectionString"]
    ?? throw new InvalidOperationException("Redis connection string is not configured");

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddSingleton<RateLimiterService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.UseMiddleware<RateLimitMiddleware>();

app.Run();