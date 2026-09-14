namespace RateLimiter.Models;

public class RateLimiterResult
{
    public bool IsAllowed { get; init; }
    public int Remaining { get; init; }
    public long RetryAfter { get; init; }
}