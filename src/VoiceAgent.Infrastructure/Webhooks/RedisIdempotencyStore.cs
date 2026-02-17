using StackExchange.Redis;

namespace VoiceAgent.Infrastructure.Webhooks;

public sealed class RedisIdempotencyStore(IConnectionMultiplexer multiplexer)
{
    private readonly IDatabase _db = multiplexer.GetDatabase();

    public async Task<bool> TryMarkProcessedAsync(string eventId, TimeSpan ttl)
    {
        return await _db.StringSetAsync($"webhook:event:{eventId}", "1", ttl, when: When.NotExists);
    }
}
