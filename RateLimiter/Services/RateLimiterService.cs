using RateLimiter.Models;
using StackExchange.Redis;

namespace RateLimiter.Services;

public class RateLimiterService(IConnectionMultiplexer connection)
{
    private readonly IDatabase _redis = connection.GetDatabase();

    private const int BucketSize = 10;
    private const long RefillIntervalMs = 200;

    private readonly string _rateLimiterScript =
        File.ReadAllText("Scripts/RateLimiter.lua");

    public async Task<RateLimiterResult> CheckAsync(string key)
    {

        var result = (RedisResult[])await _redis.ScriptEvaluateAsync(
            _rateLimiterScript,
            new RedisKey[] { key },
            new RedisValue[]
            {
                BucketSize,
                RefillIntervalMs
            });


        var allowed = (int)result[0];
        var tokenCount = (int)result[1];
        var retryAfter = (long)result[2];

        return new RateLimiterResult
        {
            IsAllowed = allowed == 1,
            Remaining = tokenCount,
            RetryAfter = retryAfter
        };
    }
}