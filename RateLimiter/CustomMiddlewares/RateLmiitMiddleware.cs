using RateLimiter.Services;

namespace RateLimiter.CustomMiddlewares;

public class RateLimitMiddleware
    (RequestDelegate next, RateLimiterService service)
{
    public async Task InvokeAsync(HttpContext context)
    {
        string apiKey = context.Request.Headers["x-api-key"].ToString();

        var result = await service.CheckAsync(apiKey);

        if (!result.IsAllowed)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(result);
            return;

        }
        
        await next(context);
    }
}