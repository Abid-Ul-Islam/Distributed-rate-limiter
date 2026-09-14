namespace RateLimiter.Models;

public class Bucket
{
    public int Count { get; set; }
    
    public DateTimeOffset LastRefill { get; set; }
}