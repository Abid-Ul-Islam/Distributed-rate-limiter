local redisTime = redis.call("TIME")

local now =
    tonumber(redisTime[1]) * 1000 +
    math.floor(tonumber(redisTime[2]) / 1000)

local bucketSize = tonumber(ARGV[1])
local refillInterval = tonumber(ARGV[2])
local tokenCount
local lastRefill

if redis.call("EXISTS", KEYS[1]) == 0 then
    tokenCount = bucketSize
    lastRefill = now
else
    tokenCount = tonumber(redis.call("HGET", KEYS[1], "Count"))
    lastRefill = tonumber(redis.call("HGET", KEYS[1], "LastRefill"))

    local elapsed = now - lastRefill
    local tokensToAdd = math.floor(elapsed / refillInterval)

    if tokensToAdd > 0 then
        tokenCount = math.min(bucketSize, tokenCount + tokensToAdd)
        lastRefill = lastRefill + (tokensToAdd * refillInterval)
    end
end

local allowed = 0

if tokenCount > 0 then
    tokenCount = tokenCount - 1
    allowed = 1
end

-- Calculate time until the next token
local elapsedSinceRefill = now - lastRefill
local retryAfter = refillInterval - (elapsedSinceRefill % refillInterval)

-- Save state
redis.call("HSET", KEYS[1],
    "Count", tokenCount,
    "LastRefill", lastRefill
)

redis.call("EXPIRE", KEYS[1], 5)

return {
    allowed,
    tokenCount,
    retryAfter
}